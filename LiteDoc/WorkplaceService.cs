using System;
using System.IO;
using System.Threading.Tasks;

public class WorkplaceService
{
    public string WorkingPath { get; init; }
    public WorkplaceService(string workingPath) => this.WorkingPath = workingPath;
    
    public Task<string> ReadText(string fileName) => File.ReadAllTextAsync(this.GetPathOf(fileName)) ?? throw new Exception("No text read.");
    public Task WriteText(string outputPathFile, string text) => File.WriteAllTextAsync(this.GetPathOf(outputPathFile), text);
    public Task WriteBytes(string outputPathFile, byte[] bytes) => File.WriteAllBytesAsync(this.GetPathOf(outputPathFile), bytes);
    public string GetPathOf(string fileName) => $"{this.WorkingPath}/{fileName}".Replace('/', Path.DirectorySeparatorChar);
    public void CreateDirectory(string path) => Directory.CreateDirectory(this.GetPathOf(path));
}