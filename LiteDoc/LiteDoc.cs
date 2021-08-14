using System.Threading.Tasks;

public static class LiteDoc
{
    public record Base(string rootPath)
    {
        public Task Run() => rootPath
            .GetConfigurations()
            .ToSections(rootPath.MovePathTo("src"))
            .WriteDocument(rootPath.MovePathTo("dist"), "output.pdf");

        public Task Watch() => rootPath
            .MovePathTo("src")
            .WatchPath(this.Run);

        // TODO creates a new space with conf file and sample init files
        public Task New() => Task.CompletedTask;
    }

    public static Base ToLiteDoc(this string rootPath) => new Base(rootPath);
}