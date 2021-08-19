using System.Collections.Generic;
using System.Threading.Tasks;

public class LiteDoc
{
    private IConfigurationService configurationService;
    private ISectionService sectionService;
    private IDocumentService documentService;
    private IFileSystemService fileSystemService;
    private IWatcher watcher;
    private LiteDocArgs args;
    private IWorkspaceService workspaceService;

    public LiteDoc(
        IConfigurationService configurationService,
        ISectionService sectionService,
        IDocumentService documentService,
        IFileSystemService fileSystemService,
        IWatcher watcher,
        LiteDocArgs args,
        IWorkspaceService workspaceService
    )
    {
        this.configurationService = configurationService;
        this.sectionService = sectionService;
        this.documentService = documentService;
        this.fileSystemService = fileSystemService;
        this.watcher = watcher;
        this.args = args;
        this.workspaceService = workspaceService;
    }

    public async Task Run()
    {
        var configurations = await this.configurationService.GetConfigurations(args.Path);
        var sections = await this.sectionService.ToSections(configurations, this.fileSystemService.MovePathTo(args.Path, "src"));
        await this.documentService.WriteDocument(sections, this.fileSystemService.MovePathTo(args.Path, "dist"), "output.pdf");
    }

    public void Watch() => this.watcher.WatchPath(this.args.Path, this.Run);
    public Task New() => this.fileSystemService.CreateWorkspace(this.args.Path, Workspace.DefaultFiles);
}

public record LiteDocArgs(string Command, string Path);