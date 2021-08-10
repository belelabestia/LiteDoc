using System;
using System.IO;
using System.Threading.Tasks;

public class WorkSpace
{
    public string WorkingPath { get; }
    public WorkSpace(string path) => this.WorkingPath = path;

    public WorkSpace MoveTo(string path) => $"{this.WorkingPath}/{path}".Replace('/', Path.DirectorySeparatorChar).ToWorkspace();

    public Task<string> ReadText() => File.ReadAllTextAsync(this.WorkingPath) ?? throw new Exception("No text read.");
    public Task Write(string text) => File.WriteAllTextAsync(this.WorkingPath, text);
    public Task Write(byte[] bytes) => File.WriteAllBytesAsync(this.WorkingPath, bytes);
    public void CreateDirectory() => Directory.CreateDirectory(this.WorkingPath);
    public Task CreateWorkspace() => Task.CompletedTask; // TODO
}

public static class WorkspaceExtensions
{
    public static WorkSpace ToWorkspace(this string workingPath) => new WorkSpace(workingPath);
}