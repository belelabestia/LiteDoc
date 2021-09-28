using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PdfSharp.Pdf;

public static class LiteDoc
{
    public static Action<IServiceCollection> LiteDocServices(Args args) => services =>
        services
            .AddHostedService<Service>()
            .Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true)
            .AddSingleton<Args>(args)
            .AddSingleton<JsonSerializerOptions>(Json.DefaultOptions)
            .AddTransient<Default>()
            .AddTransient<IJson, Json.Service>()
            .AddTransient<IConfiguration, Configuration.Service>()
            .AddTransient<ISection, Section.Service>()
            .AddTransient<IDocument, Document.Service>()
            .AddTransient<IFileSystem, FileSystem.Service>()
            .AddTransient<IWatcher, Watcher.Service>()
            .AddTransient<IParser, Parser.Service>()
            .AddTransient<IWorkspace, Workspace.Service>()
            .AddTransient<IConsole, Console.Service>();

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
        private IConsole console;

        public Default(
            IConfiguration configuration,
            ISection section,
            IDocument document,
            IFileSystem fileSystem,
            IWatcher watcher,
            Args args,
            IWorkspace workspace,
            IConsole console
        )
        {
            this.configuration = configuration;
            this.section = section;
            this.document = document;
            this.fileSystem = fileSystem;
            this.watcher = watcher;
            this.args = args;
            this.workspace = workspace;
            this.console = console;
        }

        public Task Run() =>
            args.Path
                .Effect(path => this.console.Print($"Starting LiteDoc pipeline in path: {path}"))
                .Pipe(this.configuration.GetConfiguration)
                .Effect(() => this.console.Print("Successfully fetched configuration."))
                .With(this.fileSystem.MovePathTo(args.Path, "src"))
                .Effect(() => this.console.Print("Parsing sections to PDF..."))
                .Pipe(this.section.ToSections)
                .Effect(() => this.console.Print("Successfully parsed sections."))
                .With(this.fileSystem.MovePathTo(args.Path, "dist"))
                .Pipe((PdfDocument[] secs, string distPath) => this.document.WriteDocument(secs, distPath, "output.pdf"))
                .Effect(() => this.console.Print("LiteDoc pipeline succeded."));

        public void Watch() => this.watcher.Start(this.args.Path, this.Run);
        public Task New() => this.workspace.Create(this.args.Path, Workspace.DefaultFiles);
    }

    public class Service : IHostedService
    {
        private Default liteDoc;
        private IHostApplicationLifetime lifetime;
        private Args args;

        public Service(
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




