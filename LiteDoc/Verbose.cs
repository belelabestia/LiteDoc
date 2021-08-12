using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using System.Diagnostics;

public static class _
{
    public static string Name()
    {
        var method = new StackTrace()
            .GetFrame(1)
            ?.GetMethod();

        var name = method?.Name;
        var type = method?.DeclaringType;

        return $"{type}.{name}";
    }
}

public class VerboseConfiguration : Configuration.Base, IConfiguration
{
    public new Task<IEnumerable<Configuration.Model>> GetConfigurations(string rootPath)
    {
        Console.WriteLine(_.Name());
        return base.GetConfigurations(rootPath);
    }
}

public class VerboseDocument : Document.Base, IDocument
{
    public new Task WriteDocument(PdfDocument[] sections, string outputPath, string fileName)
    {
        Console.WriteLine(_.Name());
        return base.WriteDocument(sections, outputPath, fileName);
    }
}

public class VerboseFileSystem : FileSystem.Base, IFileSystem
{
    public new Task<string> GetText(string path)
    {
        Console.WriteLine(_.Name());
        return base.GetText(path);
    }

    public new string MovePathTo(string path, string to)
    {
        Console.WriteLine(_.Name());
        return base.MovePathTo(path, to);
    }

    public new Task Save(byte[] bytes, string outputPath, string fileName)
    {
        Console.WriteLine(_.Name());
        return base.Save(bytes, outputPath, fileName);
    }

    public new Stream ToMemoryStream(byte[] pdf)
    {
        Console.WriteLine(_.Name());
        return base.ToMemoryStream(pdf);
    }
}

public class VerboseJson : Json.Base, IJson
{
    public new T Deserialize<T>(string json, JsonSerializerOptions options)
    {
        Console.WriteLine(_.Name());
        return base.Deserialize<T>(json, options);
    }
}

public class VerboseSections : Sections.Base, ISections
{
    public new string ToHtml(string content, string format)
    {
        Console.WriteLine(_.Name());
        return base.ToHtml(content, format);
    }

    public new Task<byte[]> ToPdf(string html)
    {
        Console.WriteLine(_.Name());
        return base.ToPdf(html);
    }

    public new PdfDocument ToPdfDocument(Stream stream)
    {
        Console.WriteLine(_.Name());
        return base.ToPdfDocument(stream);
    }

    public new Task<PdfDocument[]> ToSections(IEnumerable<Configuration.Model> configurations, string srcPath)
    {
        Console.WriteLine(_.Name());
        return base.ToSections(configurations, srcPath);
    }
}

public class VerboseWatcher : Watcher.Base, IWatcher
{
    public new Task WatchPath(string srcPath, Func<Task> handler)
    {
        Console.WriteLine(_.Name());
        return base.WatchPath(srcPath, handler);
    }
}