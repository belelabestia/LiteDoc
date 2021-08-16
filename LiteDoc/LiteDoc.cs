using System.Threading.Tasks;

public class LiteDoc
{
    private IConfigurationService configurationService;
    private ISectionService sectionService;
    private IDocumentService documentService;
    private IFileSystemService fileSystemService;
    private IWatcher watcher;
    private LiteDocArgs args;

    public LiteDoc(
        IConfigurationService configurationService,
        ISectionService sectionService,
        IDocumentService documentService,
        IFileSystemService fileSystemService,
        IWatcher watcher,
        LiteDocArgs args
    )
    {
        this.configurationService = configurationService;
        this.sectionService = sectionService;
        this.documentService = documentService;
        this.fileSystemService = fileSystemService;
        this.watcher = watcher;
        this.args = args;
    }

    public async Task Run()
    {
        var configurations = await this.configurationService.GetConfigurations(args.Path);
        var sections = await this.sectionService.ToSections(configurations, this.fileSystemService.MovePathTo(args.Path, "src"));
        await this.documentService.WriteDocument(sections, this.fileSystemService.MovePathTo(args.Path, "dist"), "output.pdf");
    }

    public void Watch()
    {
        var srcPath = this.fileSystemService.MovePathTo(this.args.Path, "src");
        this.watcher.WatchPath(srcPath, this.Run);
    }

    public Task New(string rootPath)
    {
        return Task.CompletedTask;
    }
}

public record LiteDocArgs(string Command, string Path);
