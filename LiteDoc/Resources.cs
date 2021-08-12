using System;
using System.Collections.Generic;

public static class Resources
{
    private static Dictionary<Type, Type> constructors;
    private static Dictionary<Type, object> cachedInstances = new Dictionary<Type, object>();

    static Resources()
    {
        constructors = new Dictionary<Type, Type>();
        constructors.Add(typeof(IConfiguration), typeof(Configuration.Base));
        constructors.Add(typeof(IDocument), typeof(Document.Base));
        constructors.Add(typeof(IFileSystem), typeof(FileSystem.Base));
        constructors.Add(typeof(IJson), typeof(Json.Base));
        constructors.Add(typeof(ISections), typeof(Sections.Base));
        constructors.Add(typeof(IWatcher), typeof(Watcher.Base));
    }

    public static T? Get<T>() => (T)cachedInstances.GetValueOrDefault(typeof(T))! ?? constructors.GetValueOrDefault(typeof(T))!.GetInstance<T>();
    public static void Use<TInterface, TClass>() where TClass : TInterface, new() => constructors[typeof(TInterface)] = typeof(TClass);
    private static T GetInstance<T>(this Type type) => (T)Activator.CreateInstance(type)!;
}