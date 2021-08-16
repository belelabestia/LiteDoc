using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var liteDoc = Host
    .CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<LiteDocArgs>(new LiteDocArgs(args[0], args[1]));
        services.AddTransient<LiteDoc>();
        services.AddTransient<IJsonService, JsonService>();
        services.AddTransient<IConfigurationService, ConfigurationService>();
        services.AddTransient<ISectionService, SectionService>();
        services.AddTransient<IDocumentService, DocumentService>();
        services.AddTransient<IFileSystemService, FileSystemService>();
        services.AddTransient<IWatcher, Watcher>();
    })
    .Build()
    .Services
    .GetService<LiteDoc>();

liteDoc!.Watch();