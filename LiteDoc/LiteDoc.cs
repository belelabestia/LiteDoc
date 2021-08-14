using System.Threading.Tasks;

public class LiteDoc
{
    private IConfigurationService configurationService;
    private ISectionService sectionService;
    private IDocumentService documentService;
    private IFileSystemService fileSystemService;
    private IWatcher watcher;

    public LiteDoc(IConfigurationService configurationService, ISectionService sectionService, IDocumentService documentService, IFileSystemService fileSystemService, IWatcher watcher)
    {
        this.configurationService = configurationService;
        this.sectionService = sectionService;
        this.documentService = documentService;
        this.fileSystemService = fileSystemService;
        this.watcher = watcher;
    }

    public async Task Run(string rootPath)
    {
        var configurations = await this.configurationService.GetConfigurations(rootPath);
        var sections = await this.sectionService.ToSections(configurations, this.fileSystemService.MovePathTo(rootPath, "src"));
        await this.documentService.WriteDocument(sections, this.fileSystemService.MovePathTo(rootPath, "dist"), "output.pdf");
    }

    public Task Watch(string rootPath)
    {
        var srcPath = this.fileSystemService.MovePathTo(rootPath, "src");
        return this.watcher.WatchPath(srcPath, this.Run);
    }

    public Task New(string rootPath)
    {
        return Task.CompletedTask;
    }
}