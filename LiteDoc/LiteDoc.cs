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

    public Task New() => this.fileSystemService.CreateWorkspace(this.args.Path, new[]
    {
        new FileDescription("first-page.html", "<h1>LiteDoc is awesome!</h1>"),
        new FileDescription("section-1.md", this.section1),
        new FileDescription("section-2.md", this.section2),
    });

    private string section1 =>
@"# Welcome to LiteDoc

This is your first _template_. You can use `html` or `markdown`.";
    private string section2 => 
@"<style>
    @page {
        @top-center {
            content: ""you can add print-specific css rules, like this"";
        }
    }
</style>

## Add new sections

You can add **all** the sections you want.

Just remember to add them in `litedoc.conf.json`!";
}

public record LiteDocArgs(string Command, string Path);