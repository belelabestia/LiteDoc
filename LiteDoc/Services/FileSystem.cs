using System.IO;
using System.Threading.Tasks;

public interface IFileSystem
{
    Task<string> GetText(string path);
    Task WriteText(string path, string text);
    string MovePathTo(string path, string to);
    Task Save(byte[] bytes, string outputPath, string fileName);
    Stream ToMemoryStream(byte[] pdf);
    void CreateDirectory(string path);
}

public static class FileSystem
{
    public class Service : IFileSystem
    {
        private IJson json;
        public Service(IJson json) => this.json = json;
        public Task<string> GetText(string path) => File.ReadAllTextAsync(path);
        public Task WriteText(string path, string text) => File.WriteAllTextAsync(path, text);
        public string MovePathTo(string path, string to) => $"{path}/{to}".Replace('/', Path.DirectorySeparatorChar);
        public Stream ToMemoryStream(byte[] pdf) => new MemoryStream(pdf);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

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
    }
}