using System;
using System.Threading.Tasks;

public record LiteDoc(string rootPath)
{
    public Task Run() => rootPath
        .GetConfigurations()
        .ToSections(rootPath.MovePathTo("src"))
        .WriteDocument(rootPath.MovePathTo("dist"), "output.pdf");

    public void StartWatching()
    {
        rootPath.MovePathTo("src").WatchPath(() => Run());
        Console.ReadLine();
    }
}

public static class LiteDocImpl
{
    public static LiteDoc ToLiteDoc(this string rootPath) => new LiteDoc(rootPath);
}