using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IWatcher
{
    void WatchPath(string srcPath, Func<Task> handler);
}

public class Watcher : IWatcher
{
    public void WatchPath(string srcPath, Func<Task> handler)
    {
        Console.WriteLine($"Started watching on path {srcPath}");
        var watcher = this.GetFileSystemWatcher(srcPath, handler);
        this.StartWatching(watcher);
    }

    private FileSystemWatcher GetFileSystemWatcher(string srcPath, Func<Task> handler)
    {
        var watcher = new FileSystemWatcher(srcPath);

        bool inUse = false;
        watcher.Changed += async (object sender, FileSystemEventArgs e) =>
        {
            if (inUse) return;
            inUse = true;
            Console.WriteLine("Starting pipeline");

            await this.WaitFileFree(e.FullPath);
            await handler();

            Console.WriteLine("Pipeline succeded.");
            inUse = false;
        };

        watcher.IncludeSubdirectories = true;
        return watcher;
    }

    private void StartWatching(FileSystemWatcher watcher)
    {
        watcher.EnableRaisingEvents = true;

        var shutdown = false;
        Console.CancelKeyPress += (sender, e) => shutdown = true;
        while (!shutdown) { }

        Console.WriteLine("LiteDoc out! See you next time :)");
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