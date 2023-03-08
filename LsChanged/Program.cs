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

            var strategy = serviceProvider.GetRequiredService<IRunnerStrategy>();

            int exitCode = strategy.Run();

            return exitCode;
        }
        catch (CommandLineParseException commandLineParseException)
        {
            bootstrapLogger.Error(commandLineParseException.Message);
            return ExitCode.InvalidCommandLine;
        }
        catch (SnapshotNotFoundException shapshotNotFoundException)
        {
            bootstrapLogger.Error(shapshotNotFoundException.Message);
            return ExitCode.SnapshotNotFound;
        }
        catch (FatalExitException fatalExitException)
        {
            bootstrapLogger.Error(fatalExitException.Message);
            return fatalExitException.ExitCode;
        }
        catch (Exception exception)
        {
#if DEBUG
            bootstrapLogger.Error("Fatal: {0}", exception);
#else
            bootstrapLogger.Error("Fatal: {0}", exception.Message);
#endif
            return ExitCode.GenericError;
        }
    }

    private static ServiceProvider CreateAndBuildServiceProvider(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton(
            sp =>
            {
                var commandLineParser = new CommandLineParser(OptionRuleProvider.Rules, o => o.SetCommand(Command.Help));
                var options = commandLineParser.Parse(args);
                return options;
            });
        services.AddSingleton(
            sp => new Func<CommandLineOptions>(() => sp.GetRequiredService<CommandLineOptions>()));

        services.AddTransient<ILoggerFactory, LoggerFactory>();
        services.AddTransient(
            sp =>
            {
                var factory = sp.GetRequiredService<ILoggerFactory>();
                var instance = factory.CreateLogger();
                return instance;
            });

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

        services.AddTransient<ScanStrategy>();
        services.AddTransient<ListStrategy>();
        services.AddTransient<CompareStrategy>();
        services.AddTransient<DeleteStrategy>();
        services.AddTransient<ClearStrategy>();

        services.AddTransient(
            sp =>
            {
                var options = sp.GetRequiredService<CommandLineOptions>();
                var command = options.Command;
                IRunnerStrategy strategy = command switch
                {
                    Command.Scan => sp.GetRequiredService<ScanStrategy>(),
                    Command.Compare => sp.GetRequiredService<CompareStrategy>(),
                    Command.List => sp.GetRequiredService<ListStrategy>(),
                    Command.Delete => sp.GetRequiredService<DeleteStrategy>(),
                    Command.Clear => sp.GetRequiredService<ClearStrategy>(),
                    _ => throw new NotSupportedException(),
                };
                return strategy;
            });

        var provider = services.BuildServiceProvider();
        return provider;
    }

    private static void Help(ILogger logger)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        
        logger.Info("LsChanged {0}", version);
        logger.Info("Copyright (c) 2023 George Harder's Centre (https://ghcentre.com)");
        logger.Info("Creates filesystem snaphots. Compares snapshots and lists files changed.");
        logger.Info(string.Empty);

        const string resourceName = "lschanged.CommandLineReference.txt";

        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Could not find resource '{resourceName}'.");
        using var reader = new StreamReader(stream);

        string content = reader.ReadToEnd();
        logger.Info(content);
    }
}