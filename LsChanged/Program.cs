using LsChanged.Collector;
using LsChanged.CommandLine;
using LsChanged.Exceptions;
using LsChanged.Logging;
using LsChanged.ProgramRunner;
using LsChanged.Store;
using LsChanged.Store.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LsChanged;

internal static class Program
{
    private static ServiceProvider CreateAndBuildServiceProvider(string[] args)
    {
        var services = new ServiceCollection();

        services.AddTransient<ILoggerFactory, LoggerFactory>();
        services.AddTransient<ILogger>(
            sp =>
            {
                var factory = sp.GetRequiredService<ILoggerFactory>();
                var instance = factory.CreateLogger();
                return instance;
            });

        services.AddSingleton(
            sp =>
            {
                var commandLineParser = new CommandLineParser(OptionRuleProvider.Rules, o => o.SetCommand(Command.Help));
                var options = commandLineParser.Parse(args);
                return options;
            });
        services.AddSingleton(
            sp => new Func<CommandLineOptions>(() => sp.GetRequiredService<CommandLineOptions>()));

        services.AddTransient<IStoreRecordFactory, StoreRecordFactory>();

        services.AddTransient<IStoreRecordDeserializer, StoreRecordDeserializer>();
        services.AddTransient<IStoreRecordSerializer, StoreRecordSerializer>();
        services.AddTransient<IStoreRecordReader, StoreRecordReader>();
        services.AddTransient<IStoreRecordWriter, StoreRecordWriter>();
        services.AddTransient<ICurrentTimeProvider, CurrentTimeProvider>();

        services.AddSingleton<IStore>(
            sp =>
            {
                var options = sp.GetRequiredService<CommandLineOptions>();
                var instance = new Store.Store(
                    options.StorePath!,
                    sp.GetRequiredService<ICurrentTimeProvider>(),
                    sp.GetRequiredService<IStoreRecordReader>(),
                    sp.GetRequiredService<IStoreRecordWriter>());
                return instance;
            });

        services.AddSingleton(
            sp =>
            new Func<CommandLineOptions, IFileInfoCollector>(
                o =>
                new FileInfoCollector(sp.GetRequiredService<ILogger>(), o.FollowSymlinksMode)));

        services.AddSingleton(
            sp => new Func<ScanStrategy>(
                () =>
                {
                    var instance = new ScanStrategy(
                        sp.GetRequiredService<ILogger>(),
                        sp.GetRequiredService<CommandLineOptions>(),
                        sp.GetRequiredService<IStore>(),
                        sp.GetRequiredService<Func<CommandLineOptions, IFileInfoCollector>>(),
                        sp.GetRequiredService<IStoreRecordFactory>(),
                        sp.GetRequiredService<ICurrentTimeProvider>());
                    return instance;
                }));
        services.AddTransient<ListStrategy>(); // TODO: replace with Func for stragegy impl
        services.AddTransient<CompareStrategy>(); // TODO: replace with Func for stragegy impl

        var provider = services.BuildServiceProvider();
        return provider;
    }

    private static int Main(string[] args)
    {
        var serviceProvider = CreateAndBuildServiceProvider(args);

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var bootstrapLogger = loggerFactory.CreateBootstrapLogger();

        try
        {
            var options = serviceProvider.GetRequiredService<CommandLineOptions>();
            var logger = loggerFactory.CreateLogger();

            //
            // help does not require IStore and '-s' switch, so we handle it specifically
            //
            if (options.Command == Command.Help)
            {
                Help(bootstrapLogger);
                return ExitCode.HelpDisplayed;
            }

            options.Validate();

            var store = serviceProvider.GetRequiredService<IStore>();

            IRunnerStrategy strategy = options.Command switch
            {
                Command.Scan => serviceProvider.GetRequiredService<Func<ScanStrategy>>()(),
                Command.Compare => serviceProvider.GetRequiredService<CompareStrategy>(),
                Command.List => serviceProvider.GetRequiredService<ListStrategy>(),
                Command.Delete => throw new NotImplementedException(),
                Command.Clear => throw new NotImplementedException(),
                _ => throw new NotSupportedException(),
            };

            int exitCode = strategy.Run();
            return exitCode;
        }
        catch (CommandLineParseException commandLineParseException)
        {
            bootstrapLogger.Error(commandLineParseException.Message);
            return ExitCode.InvalidCommandLine;
        }
        catch (FatalExitException fatalExitException)
        {
            bootstrapLogger.Error(fatalExitException.Message);
            return fatalExitException.ExitCode;
        }
        catch (Exception exception)
        {
            bootstrapLogger.Error("Fatal: {0}", exception);
            return ExitCode.GenericError;
        }
    }

    private static void Help(ILogger logger)
    {
        const string resourceName = "LsChanged.CommandLineReference.txt";
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Could not find resource '{resourceName}'.");
        using var reader = new StreamReader(stream);

        string content = reader.ReadToEnd();
        logger.Info(content);
    }
}