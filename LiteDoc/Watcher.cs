using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IWatcher
{
    Task WatchPath(string srcPath, Func<Task> handler);
}

public static class Watcher
{
    public static IWatcher Instance = new Default();
    public class Default : IWatcher
    {
        public Task WatchPath(string srcPath, Func<Task> handler)
        {
            Console.WriteLine($"Started watching on path {srcPath}");
            return srcPath
                .GetFileSystemWatcher(handler)
                .StartWatching();
        }
    }
    public static Task WatchPath(this string srcPath, Func<Task> handler) => Instance.WatchPath(srcPath, handler);

    private static FileSystemWatcher GetFileSystemWatcher(this string srcPath, Func<Task> handler)
    {
        var watcher = new FileSystemWatcher(srcPath);

        bool inUse = false;
        watcher.Changed += async (object sender, FileSystemEventArgs e) =>
        {
            if (inUse) return;
            inUse = true;
            Console.WriteLine("Starting pipeline");

            await e.FullPath.WaitFileFree();
            await handler();

            Console.WriteLine("Pipeline succeded.");
            inUse = false;
        };

        watcher.IncludeSubdirectories = true;
        return watcher;
    }
    private static Task StartWatching(this FileSystemWatcher watcher)
    {
        watcher.EnableRaisingEvents = true;
        return Task.Delay(Timeout.Infinite);
    }

    private static Task WaitFileFree(this string path) => Task.Run(() =>
    {
        while (!IsFileFree(path)) { }
        return;
    });

    private static bool IsFileFree(this string path)
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