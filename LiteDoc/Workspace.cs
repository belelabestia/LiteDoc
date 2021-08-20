using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public interface IWorkspaceService
{
    Task Create(string rootPath, IEnumerable<FileDescription> files);
    // void Apply(/*configuration*/);
}

public class WorkserService : IWorkspaceService
{
    private IFileSystem fileSystemService;
    private IJson jsonService;

    public WorkserService(
        IFileSystem fileSystemService,
        IJson jsonService
    )
    {
        this.fileSystemService = fileSystemService;
        this.jsonService = jsonService;
    }

    public async Task Create(string rootPath, IEnumerable<FileDescription> files)
    {
        this.fileSystemService.CreateDirectory(rootPath);

        var configurations = files
            .AsParallel()
            .AsOrdered()
            .Select(file =>
            {
                var format = file.Path.Split('.').Last();
                return new Configuration.Model(file.Path, format);
            })
            .ToList();

        var json = this.jsonService.Serialize(configurations);
        var confFileName = this.fileSystemService.MovePathTo(rootPath, Configuration.DefaultFileName);
        await this.fileSystemService.WriteText(confFileName, json);
        var srcPath = this.fileSystemService.MovePathTo(rootPath, "src");
        this.fileSystemService.CreateDirectory(srcPath);

        var tasks = files
            .AsParallel()
            .AsOrdered()
            .Select(file =>
            {
                var filePath = this.fileSystemService.MovePathTo(srcPath, file.Path);
                var dir = this.fileSystemService.MovePathTo(filePath, "..");
                this.fileSystemService.CreateDirectory(dir);

                return this.fileSystemService.WriteText(filePath, file.Content);
            });

        await Task.WhenAll(tasks);
    }
}

public static class Workspace
{
    public static IEnumerable<FileDescription> DefaultFiles => new[]
    {
        new FileDescription("first-page.html", "<h1>LiteDoc is awesome!</h1>"),
        new FileDescription("section-1.md", section1),
        new FileDescription("section-2.md", section2),
    };

    private static string section1 =>
@"# Welcome to LiteDoc

This is your first _template_. You can use `html` or `markdown`.";
    private static string section2 =>
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

public record FileDescription(string Path, string Content);
