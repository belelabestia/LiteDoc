using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .UseConsoleLifetime()
    .ConfigureHostConfiguration(conf =>
    {
        conf.AddCommandLine(args);
    })
    .ConfigureServices(services =>
    {
        services.AddTransient<LiteDoc>();
        services.AddTransient<IConfigurationService, ConfigurationService>();
        services.AddTransient<ISectionService, SectionService>();
        services.AddTransient<IDocumentService, DocumentService>();
        services.AddTransient<IFileSystemService, FileSystemService>();
        services.AddTransient<IWatcher, Watcher>();
    });

public class LiteDocConsoleService : BackgroundService, IHostedService
{
    private string command;
    private string rootPath;
    private LiteDoc liteDoc;

    public LiteDocConsoleService(string command, string rootPath, LiteDoc liteDoc)
    {
        this.command = command;
        this.rootPath = rootPath;
        this.liteDoc = liteDoc;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => this.command switch
    {
        "run" => this.liteDoc.Run(this.rootPath),
        "watch" => this.liteDoc.Watch(this.rootPath),
        "new" => this.liteDoc.New(this.rootPath),
        _ => throw new Exception("Invalid litedoc command: run, watch, new.")
    };
}