using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host
    .CreateDefaultBuilder()
    .ConfigureLogging(logging => logging.AddSimpleConsole(options => options.SingleLine = true))
    .UseConsoleLifetime()
    .ConfigureServices(services =>
    {
        services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
        services.AddHostedService<LiteDocService>();

        var typedArgs = new LiteDocArgs(args[0], args[1]);
        services.AddSingleton<ICommand>(typedArgs);
        services.AddSingleton<IPath>(typedArgs);
        services.AddTransient<LiteDoc>();
        services.AddTransient<IJsonService, JsonService>();
        services.AddTransient<IConfigurationService, ConfigurationService>();
        services.AddTransient<ISectionService, SectionService>();
        services.AddTransient<IDocumentService, DocumentService>();
        services.AddTransient<IFileSystemService, FileSystemService>();
        services.AddTransient<IWatcher, Watcher>();
    })
    .Build();

await host.RunAsync();