using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public interface IFileSystemService
{
    Task<string> GetText(string path);
    string MovePathTo(string path, string to);
    Task Save(byte[] bytes, string outputPath, string fileName);
    Stream ToMemoryStream(byte[] pdf);
    Task CreateWorkspace(string rootPath, IEnumerable<FileDescription> files);
}

public class FileSystemService : IFileSystemService
{
    public Task<string> GetText(string path) => File.ReadAllTextAsync(path);
    public string MovePathTo(string path, string to) => $"{path}/{to}".Replace('/', Path.DirectorySeparatorChar);
    public Stream ToMemoryStream(byte[] pdf) => new MemoryStream(pdf);

    public Task Save(byte[] bytes, string outputPath, string fileName)
    {
        this.Clear(outputPath);
        return File.WriteAllBytesAsync(this.MovePathTo(outputPath, fileName), bytes);
    }

    public void Clear(string path)
    {
        if (Directory.Exists(path)) Directory.Delete(path, true);
        Directory.CreateDirectory(path);
    }

    public async Task CreateWorkspace(string rootPath, IEnumerable<FileDescription> files)
    {
        Directory.CreateDirectory(rootPath);

        var configurations = files
            .AsParallel()
            .Select(file =>
            {
                var format = file.Path.Split('.').Last();
                return new Configuration(file.Path, format);
            })
            .ToList();

        var json = JsonSerializer.Serialize(configurations, Json.DefaultOptions);
        var confFileName = this.MovePathTo(rootPath, "litedoc.conf.json");
        await File.WriteAllTextAsync(confFileName, json);
        var srcPath = this.MovePathTo(rootPath, "src");
        Directory.CreateDirectory(srcPath);

        var tasks = files
            .AsParallel()
            .Select(file =>
            {
                var filePath = this.MovePathTo(srcPath, file.Path);
                var dir = this.MovePathTo(filePath, "..");
                Directory.CreateDirectory(dir);

                return File.WriteAllTextAsync(filePath, file.Content);
            });

        await Task.WhenAll(tasks);
    }
}

public record FileDescription(string Path, string Content);