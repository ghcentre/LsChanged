using LsChanged.Collector;
using LsChanged.CommandLine;
using LsChanged.Exceptions;
using LsChanged.Logging;
using LsChanged.ProgramRunner;
using LsChanged.Store;
using LsChanged.Store.Abstractions;
using Microsoft.Extensions.DependencyInjection;

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
            options.Validate();

            var strategy = serviceProvider.GetRequiredService<IRunnerStrategy>();

            int exitCode = strategy.Run();
            return exitCode;
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

        services.AddTransient<HelpStrategy>();
        services.AddTransient<ScanStrategy>();
        services.AddTransient<ListStrategy>();
        services.AddTransient<CompareStrategy>();
        services.AddTransient<DeleteStrategy>();
        services.AddTransient<ClearStrategy>();
        services.AddTransient<NewIgnoreStrategy>();

        services.AddTransient(
            sp =>
            {
                var options = sp.GetRequiredService<CommandLineOptions>();
                var command = options.Command;
                IRunnerStrategy strategy = command switch
                {
                    Command.Help => sp.GetRequiredService<HelpStrategy>(),
                    Command.Scan => sp.GetRequiredService<ScanStrategy>(),
                    Command.Compare => sp.GetRequiredService<CompareStrategy>(),
                    Command.List => sp.GetRequiredService<ListStrategy>(),
                    Command.Delete => sp.GetRequiredService<DeleteStrategy>(),
                    Command.Clear => sp.GetRequiredService<ClearStrategy>(),
                    Command.NewIgnore => sp.GetRequiredService<NewIgnoreStrategy>(),
                    _ => throw new NotSupportedException(),
                };
                return strategy;
            });

        var provider = services.BuildServiceProvider();
        return provider;
    }
}