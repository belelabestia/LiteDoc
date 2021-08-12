using System.IO;
using System.Threading.Tasks;

public interface IFileSystem
{
    Task<string> GetText(string path);
    string MovePathTo(string path, string to);
    Task Save(byte[] bytes, string outputPath, string fileName);
    Stream ToMemoryStream(byte[] pdf);
}

public static class FileSystem
{
    public class Base : IFileSystem
    {
        public Task<string> GetText(string path) => File.ReadAllTextAsync(path);
        public string MovePathTo(string path, string to) => $"{path}/{to}".Replace('/', Path.DirectorySeparatorChar);
        public Task Save(byte[] bytes, string outputPath, string fileName)
        {
            outputPath.Clear();
            return File.WriteAllBytesAsync(outputPath.MovePathTo(fileName), bytes);
        }
        public Stream ToMemoryStream(byte[] pdf) => new MemoryStream(pdf);
    }
    public static Task<string> GetText(this string path) => Resources.Get<IFileSystem>()!.GetText(path);
    public static string MovePathTo(this string path, string to) => Resources.Get<IFileSystem>()!.MovePathTo(path, to);
    public static Task Save(this byte[] bytes, string outputPath, string fileName) => Resources.Get<IFileSystem>()!.Save(bytes, outputPath, fileName);
    public static Stream ToMemoryStream(this byte[] pdf) => Resources.Get<IFileSystem>()!.ToMemoryStream(pdf);
    public static void Clear(this string outputPath)
    {
        if (Directory.Exists(outputPath)) Directory.Delete(outputPath, true);
        Directory.CreateDirectory(outputPath);
    }
}