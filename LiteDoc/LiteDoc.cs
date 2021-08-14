using System.Threading.Tasks;

public record LiteDoc(string rootPath)
{
    public async Task Run()
    {
        await rootPath
            .GetConfigurations()
            .ToSections(rootPath.MovePathTo("src"))
            .WriteDocument(rootPath.MovePathTo("dist"), "output.pdf");
    }

    public async Task StartWatching() => await rootPath.MovePathTo("src").WatchPath(this.Run);
}

public static class LiteDocImpl
{
    public static LiteDoc ToLiteDoc(this string rootPath) => new LiteDoc(rootPath);
}