using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using PdfSharp.Pdf;

public static class LiteDoc
{
    public static Action<IServiceCollection> LiteDocServices(Args args) => services =>
        services
            .AddHostedService<Console>()
            .Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true)
            .AddSingleton<Args>(args)
            .AddSingleton<JsonSerializerOptions>(Json.DefaultOptions)
            .AddTransient<Default>()
            .AddTransient<IJson, Json.Service>()
            .AddTransient<IConfiguration, Configuration.Service>()
            .AddTransient<ISection, Section.Service>()
            .AddTransient<IDocument, Document.Service>()
            .AddTransient<IFileSystem, FileSystem.Service>()
            .AddTransient<IWatcher, Watcher.Service>();

    public static IHostBuilder UseLiteDocConsole(this IHostBuilder builder, string[] args) =>
        builder
            .UseConsoleLifetime()
            .ConfigureServices(LiteDocServices(new Args(args[0], args[1])));

    public static Task RunLiteDocConsole(this string[] args) =>
        Host
            .CreateDefaultBuilder()
            .UseLiteDocConsole(args)
            .RunConsoleAsync();

    public record Args(string Command, string Path);

    public class Default
    {
        private IConfiguration configuration;
        private ISection section;
        private IDocument document;
        private IFileSystem fileSystem;
        private IWatcher watcher;
        private Args args;
        private IWorkspace workspace;

        public Default(
            IConfiguration configuration,
            ISection section,
            IDocument document,
            IFileSystem fileSystem,
            IWatcher watcher,
            Args args,
            IWorkspace workspace
        )
        {
            this.configuration = configuration;
            this.section = section;
            this.document = document;
            this.fileSystem = fileSystem;
            this.watcher = watcher;
            this.args = args;
            this.workspace = workspace;
        }

        public Task Run() =>
            args.Path
                .Pipe(this.configuration.GetConfiguration)
                .Pipe(this.ToSections(this.fileSystem.MovePathTo(args.Path, "src")))
                .Pipe(this.WriteDocument(this.fileSystem.MovePathTo(args.Path, "dist"), "output.pdf"));

        public void Watch() => this.watcher.Start(this.args.Path, this.Run);
        public Task New() => this.workspace.Create(this.args.Path, Workspace.DefaultFiles);

        private Func<IEnumerable<Configuration.Model>, Task<PdfDocument[]>> ToSections(string srcPath) =>
            configurations => this.section.ToSections(configurations, srcPath);

        private Func<PdfDocument[], Task> WriteDocument(string outputPath, string fileName) =>
            sections => this.document.WriteDocument(sections, outputPath, fileName);
    }

    public class Console : IHostedService
    {
        private Default liteDoc;
        private IHostApplicationLifetime lifetime;
        private Args args;

        public Console(
            Default liteDoc,
            IHostApplicationLifetime lifetime,
            Args args
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
}




