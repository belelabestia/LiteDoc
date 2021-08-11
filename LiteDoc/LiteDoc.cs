using System.Threading.Tasks;

// Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

public class LiteDoc
{
    public Task Run(string rootPath) => rootPath
        .GetConfigurations()
        .ToSections(rootPath.MovePathTo("src"))
        .WriteDocument(rootPath.MovePathTo("dist/output.pdf"));
}