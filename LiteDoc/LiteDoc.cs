using System;
using System.Threading.Tasks;

public class LiteDoc
{
    private IConfigurationService configurationService;
    private ISectionService sectionService;
    private IDocumentService documentService;
    private IFileSystemService fileSystemService;
    private IWatcher watcher;
    private IPath path;

    public LiteDoc(
        IConfigurationService configurationService,
        ISectionService sectionService,
        IDocumentService documentService,
        IFileSystemService fileSystemService,
        IWatcher watcher,
        IPath path
    )
    {
        this.configurationService = configurationService;
        this.sectionService = sectionService;
        this.documentService = documentService;
        this.fileSystemService = fileSystemService;
        this.watcher = watcher;
        this.path = path;
    }

    public async Task Run()
    {
        var configurations = await this.configurationService.GetConfigurations(path.WorkspacePath);
        var sections = await this.sectionService.ToSections(configurations, this.fileSystemService.MovePathTo(path.WorkspacePath, "src"));
        await this.documentService.WriteDocument(sections, this.fileSystemService.MovePathTo(path.WorkspacePath, "dist"), "output.pdf");
    }

    public void Watch()
    {
        var srcPath = this.fileSystemService.MovePathTo(this.path.WorkspacePath, "src");
        this.watcher.WatchPath(srcPath, this.Run);
    }

    public Task New()
    {
        return Task.CompletedTask;
    }
}