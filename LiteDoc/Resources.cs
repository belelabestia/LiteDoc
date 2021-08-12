public static class Resources
{
    public static T? Get<T>() => typeof(T) switch
    {
        IConfiguration => (T)Configurations.Instance,
        IDocument => (T)Document.Instance,
        IFileSystem => (T)FileSystem.Instance,
        IJson => (T)Json.Instance,
        ISections => (T)Sections.Instance,
        IWatcher => (T)Watcher.Instance,
        _ => default(T)
    };

    public static void Use(IConfiguration instance) => Configurations.Instance = instance;
    public static void Use(IDocument instance) => Document.Instance = instance;
    public static void Use(IFileSystem instance) => FileSystem.Instance = instance;
    public static void Use(IJson instance) => Json.Instance = instance;
    public static void Use(ISections instance) => Sections.Instance = instance;
    public static void Use(IWatcher instance) => Watcher.Instance = instance;
}