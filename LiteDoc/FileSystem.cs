using System.IO;
using System.Threading.Tasks;

public interface IFileSystem
{
    Task<string> GetText(string path);
    string MovePathTo(string path, string to);
    Task Save(byte[] bytes, string fileName);
    Stream ToMemoryStream(byte[] pdf);
}

public static class FileSystem
{
    public static IFileSystem Instance = new Default();
    public class Default : IFileSystem
    {
        public Task<string> GetText(string path) => File.ReadAllTextAsync(path);
        public string MovePathTo(string path, string to) => $"{path}/{to}".Replace('/', Path.DirectorySeparatorChar);
        public Task Save(byte[] bytes, string fileName) => File.WriteAllBytesAsync(fileName, bytes);
        public Stream ToMemoryStream(byte[] pdf) => new MemoryStream(pdf);
    }
    public static Task<string> GetText(this string path) => Instance.GetText(path);
    public static string MovePathTo(this string path, string to) => Instance.MovePathTo(path, to);
    public static Task Save(this byte[] bytes, string fileName) => Instance.Save(bytes, fileName);
    public static Stream ToMemoryStream(this byte[] pdf) => Instance.ToMemoryStream(pdf);
}