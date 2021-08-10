using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public static class LiteDocExtensions
{
    public static WorkSpace GetWorkSpace(this string workingPath) => workingPath.ToWorkspace();
    public static Json GetJson(this string workingPath) => workingPath.GetWorkSpace().ToJson();
    public static Configuration GetConfiguration(this string workingPath) => workingPath.GetJson().ToConfiguration();
    public static Section GetSection(this string workingPath) => (workingPath.GetConfiguration(), workingPath.GetWorkSpace()).ToSection();
    public static Parser GetParser(this string workingPath) => (workingPath.GetConfiguration(), workingPath.GetSection()).ToParser();
    public static Html GetHtml(this string workingPath) => (workingPath.GetWorkSpace(), workingPath.GetParser()).ToHtml();
    public static PdfWriter GetPdfWriter(this string workingPath) => (workingPath.GetConfiguration(), workingPath.GetWorkSpace(), workingPath.GetHtml()).ToPdfWriter();
}

public class LiteDoc
{
    private string workingPath;
    public LiteDoc(string workingPath) => this.workingPath = workingPath;
    public async Task RunOnce()
    {
        await workingPath.GetPdfWriter().WriteDocument();
    }
    public void StartWatching()
    {
        FileSystemEventHandler stocazzo = async (object sender, FileSystemEventArgs e) =>
        {
            Console.WriteLine(e.ChangeType);
            Console.WriteLine("Beginning cycle.");
            Thread.Sleep(100);
            await RunOnce();
            Console.WriteLine("End of cycle.");
        };

        var watcher = new FileSystemWatcher
        {
            Path = this.workingPath,
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
        };

        watcher.Changed += stocazzo;
        RunOnce().Wait();

        Console.ReadLine();
    }
    // public Task CreateWorkspace() { } // TODO
}

public static class LiteDocDefault
{
    public const string ConfigurationFileName = "litedoc.conf.json";
    public static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public static LiteDoc ToLiteDoc(this string workingPath) => new LiteDoc(workingPath);
}