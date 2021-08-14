using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IWatcher
{
    Task WatchPath(string srcPath, Func<string, Task> handler);
}

public class Watcher : IWatcher
{
    public Task WatchPath(string srcPath, Func<string, Task> handler)
    {
        Console.WriteLine($"Started watching on path {srcPath}");

        var watcher = this.GetFileSystemWatcher(srcPath, handler);
        return this.StartWatching(watcher);
    }

    private FileSystemWatcher GetFileSystemWatcher(string srcPath, Func<string, Task> handler)
    {
        var watcher = new FileSystemWatcher(srcPath);

        bool inUse = false;
        watcher.Changed += async (object sender, FileSystemEventArgs e) =>
        {
            if (inUse) return;
            inUse = true;
            Console.WriteLine("Starting pipeline");

            await this.WaitFileFree(e.FullPath);
            await handler(srcPath);

            Console.WriteLine("Pipeline succeded.");
            inUse = false;
        };

        watcher.IncludeSubdirectories = true;
        return watcher;
    }

    private Task StartWatching(FileSystemWatcher watcher)
    {
        watcher.EnableRaisingEvents = true;
        return Task.Delay(Timeout.Infinite);
    }

    private Task WaitFileFree(string path) => Task.Run(() =>
    {
        while (!this.IsFileFree(path)) { }
        return;
    });

    private bool IsFileFree(string path)
    {
        try
        {
            using var inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return inputStream.Length > 0;
        }
        catch
        {
            return false;
        }
    }
}