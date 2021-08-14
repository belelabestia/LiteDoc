using System;
using System.Collections.Generic;

public static class Resources
{
    private static Dictionary<Type, Type> constructors = new Dictionary<Type, Type>()
    {
        [typeof(IConfiguration)] = typeof(Configuration.Base),
        [typeof(IDocument)] = typeof(Document.Base),
        [typeof(IFileSystem)] = typeof(FileSystem.Base),
        [typeof(IJson)] = typeof(Json.Base),
        [typeof(ISections)] = typeof(Sections.Base),
        [typeof(IWatcher)] = typeof(Watcher.Base)
    };

    public static T? Get<T>() => constructors.GetValueOrDefault(typeof(T))!.GetInstance<T>();
    public static void Use<TInterface, TClass>() where TClass : TInterface, new() => constructors[typeof(TInterface)] = typeof(TClass);
    private static T GetInstance<T>(this Type type) => (T)Activator.CreateInstance(type)!;
}