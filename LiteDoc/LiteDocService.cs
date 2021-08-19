using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class LiteDocService : IHostedService
{
    private LiteDoc liteDoc;
    private IHostApplicationLifetime lifetime;
    private LiteDocArgs args;

    public LiteDocService(
        LiteDoc liteDoc,
        IHostApplicationLifetime lifetime,
        LiteDocArgs args
    )
    {
        this.liteDoc = liteDoc;
        this.lifetime = lifetime;
        this.args = args;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        switch (this.args.Command)
        {
            case "run":
                await this.liteDoc.Run();
                this.lifetime.StopApplication();
                return;
            case "watch":
                this.liteDoc.Watch();
                return;
            case "new":
                await this.liteDoc.New();
                this.lifetime.StopApplication();
                return;
            default:
                throw new Exception("Invalid command; usage: litedoc [run | watch | new] <workspace-path>");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public static class LiteDocExtensions
{
    public static IHostBuilder UseLiteDoc(this IHostBuilder builder, string[] args) => builder
        .UseConsoleLifetime()
        .ConfigureServices(services =>
        {
            services.AddHostedService<LiteDocService>();
            services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

            var typedArgs = new LiteDocArgs(args[0], args[1]);
            services.AddSingleton<LiteDocArgs>(typedArgs);
            services.AddSingleton<JsonSerializerOptions>(Json.DefaultOptions);
            services.AddTransient<LiteDoc>();
            services.AddTransient<IJsonService, JsonService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<ISectionService, SectionService>();
            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IFileSystemService, FileSystemService>();
            services.AddTransient<IWatcher, Watcher>();
        });

    public static Task RunLiteDoc(this string[] args) => Host
        .CreateDefaultBuilder()
        .UseLiteDoc(args)
        .RunConsoleAsync();
}