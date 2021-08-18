using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface IWatcher
{
    void WatchPath(string rootPath, Func<Task> handler);
}

public class Watcher : IWatcher
{
    private IFileSystemService fileSystemService;
    public Watcher(IFileSystemService fileSystemService) => this.fileSystemService = fileSystemService;

    public void WatchPath(string rootPath, Func<Task> handler)
    {
        var confWatcher = new FileSystemWatcher
        {
            Path = rootPath,
            Filter = "litedoc.conf.json"
        };

        var srcPath = this.fileSystemService.MovePathTo(rootPath, "src");

        var srcWatcher = new FileSystemWatcher
        {
            Path = srcPath,
            IncludeSubdirectories = true
        };

        this.StartWatching(new[] { confWatcher, srcWatcher }, handler);
    }

    private void StartWatching(IEnumerable<FileSystemWatcher> watchers, Func<Task> handler)
    {
        bool inUse = false;
        foreach (var watcher in watchers)
        {
            watcher.Changed += async (object sender, FileSystemEventArgs e) =>
            {
                if (inUse) return;
                inUse = true;

                await this.WaitFileFree(e.FullPath);

                try
                {
                    await handler();
                }
                catch { }

                inUse = false;
            };

            watcher.EnableRaisingEvents = true;
        }
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