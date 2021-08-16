using System.IO;
using System.Threading.Tasks;

public interface IFileSystemService
{
    Task<string> GetText(string path);
    string MovePathTo(string path, string to);
    Task Save(byte[] bytes, string outputPath, string fileName);
    Stream ToMemoryStream(byte[] pdf);
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

    public void Clear(string outputPath)
    {
        if (Directory.Exists(outputPath)) Directory.Delete(outputPath, true);
        Directory.CreateDirectory(outputPath);
    }
}
