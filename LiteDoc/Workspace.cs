using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public interface IWorkspace
{
    Task Create(string rootPath, IEnumerable<Workspace.File> files);
    // void Apply(/*configuration*/);
}

public static class Workspace
{
    public class Service : IWorkspace
    {
        private IFileSystem fileSystemService;
        private IJson jsonService;

        public Service(
            IFileSystem fileSystemService,
            IJson jsonService
        )
        {
            this.fileSystemService = fileSystemService;
            this.jsonService = jsonService;
        }

        public async Task Create(string rootPath, IEnumerable<File> files)
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

    public static IEnumerable<File> DefaultFiles => new[]
    {
        new File("first-page.html", "<h1>LiteDoc is awesome!</h1>"),
        new File("section-1.md", section1),
        new File("section-2.md", section2),
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
    public record File(string Path, string Content);
}