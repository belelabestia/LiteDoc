using System.IO;
using System.Threading.Tasks;
using OpenQA.Selenium.Edge;

public record LiteDoc(string rootPath)
{
    public async Task Run()
    {
        await rootPath
            .GetConfigurations()
            .ToSections(rootPath.MovePathTo("src"))
            .WriteDocument(rootPath.MovePathTo("dist"), "output.pdf");

    }

    public async Task StartWatching()
    {
        var outputFile = rootPath.MovePathTo("dist/output.pdf");
        var fullPath = Path.GetFullPath(outputFile);
        var replace = fullPath.Replace('\\', '/');
        var url = $"file:///{replace}";

        var options = new EdgeOptions { UseChromium = true };
        var driver = new EdgeDriver(options) { Url = url };

        await rootPath.MovePathTo("src").WatchPath(async () =>
        {
            await this.Run();
            try
            {
                driver.Navigate().Refresh();
            }
            catch
            {
                driver = new EdgeDriver(options) { Url = url };
            }
        });
    }
}

public static class LiteDocImpl
{
    public static LiteDoc ToLiteDoc(this string rootPath) => new LiteDoc(rootPath);
}