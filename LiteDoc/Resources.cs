using System;
using System.Collections.Generic;

public static class Resources
{
    private static Dictionary<Type, Type> instances;

    static Resources()
    {
        instances = new Dictionary<Type, Type>();
        instances.Add(typeof(IConfiguration), typeof(Configuration.Base));
        instances.Add(typeof(IDocument), typeof(Document.Base));
        instances.Add(typeof(IFileSystem), typeof(FileSystem.Base));
        instances.Add(typeof(IJson), typeof(Json.Base));
        instances.Add(typeof(ISections), typeof(Sections.Base));
        instances.Add(typeof(IWatcher), typeof(Watcher.Base));
    }

    public static T? Get<T>() => instances.GetValueOrDefault(typeof(T))!.GetInstance<T>();
    public static void Use<TInterface, TClass>() where TClass : TInterface, new() => instances[typeof(TInterface)] = typeof(TClass);
    private static T GetInstance<T>(this Type type) => (T)Activator.CreateInstance(type)!;
}