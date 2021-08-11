using System;

public interface IWatcher
{
    void WatchPath(string srcPath, Action handler);
}

public static class Watcher
{
    public static Default Instance = new Default();
    public class Default : IWatcher
    {
        public void WatchPath(string srcPath, Action handler)
        {
            handler();
        }
    }
    public static void WatchPath(this string srcPath, Action handler) => Instance.WatchPath(srcPath, handler);
}