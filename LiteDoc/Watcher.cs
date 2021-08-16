using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public interface IWatcher
{
    void WatchPath(string srcPath, Func<Task> handler);
}

public class Watcher : IWatcher
{
    private IHostApplicationLifetime lifetime;

    public Watcher(IHostApplicationLifetime lifetime) => this.lifetime = lifetime;

    public void WatchPath(string srcPath, Func<Task> handler)
    {
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

            await this.WaitFileFree(e.FullPath);
            await handler();

            inUse = false;
        };

        watcher.IncludeSubdirectories = true;
        return watcher;
    }

    private void StartWatching(FileSystemWatcher watcher) => watcher.EnableRaisingEvents = true;

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