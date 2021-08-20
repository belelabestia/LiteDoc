using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface IWatcher
{
    void Start(string rootPath, Func<Task> handler);
}

public static class Watcher
{
    public class Service : IWatcher
    {
        private IFileSystem fileSystem;
        public Service(IFileSystem fileSystem) => this.fileSystem = fileSystem;

        public void Start(string rootPath, Func<Task> handler)
        {
            var confWatcher = new FileSystemWatcher
            {
                Path = rootPath,
                Filter = "litedoc.conf.json"
            };

            var srcWatcher = new FileSystemWatcher
            {
                Path = rootPath.Pipe(this.MovePathTo("src")),
                IncludeSubdirectories = true
            };

            this.Activate(new[] { confWatcher, srcWatcher }, handler);
        }

        private void Activate(IEnumerable<FileSystemWatcher> watchers, Func<Task> handler)
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

        private Func<string, string> MovePathTo(string to) =>
            path => this.fileSystem.MovePathTo(path, to);
    }
}