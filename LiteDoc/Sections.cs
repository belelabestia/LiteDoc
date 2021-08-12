using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Balbarak.WeasyPrint;
using Markdig;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public interface ISections
{
    string ToHtml(string content, string format);
    Task<byte[]> ToPdf(string html);
    Task<PdfDocument[]> ToSections(IEnumerable<Configuration.Model> configurations, string srcPath);
    PdfDocument ToPdfDocument(Stream stream);
}

public static class Sections
{
    public class Base : ISections
    {
        public Task<PdfDocument[]> ToSections(IEnumerable<Configuration.Model> configurations, string srcPath) => configurations
            .Select(configuration => srcPath
                .MovePathTo(configuration.Path)
                .GetText()
                .FlatMap(text => text.ToHtml(configuration.Format).ToPdf())
                .Map(pdf => pdf.ToMemoryStream().ToPdfDocument())
            )
            .ToTask();

        public string ToHtml(string content, string format) => format switch
        {
            "html" => content + Environment.NewLine,
            "md" => Markdown.ToHtml(content),
            _ => throw new Exception("Invalid format.")
        };

        public Task<byte[]> ToPdf(string html)
        {
            using var weasy = new WeasyPrintClient();
            return weasy.GeneratePdfAsync(html);
        }

        public PdfDocument ToPdfDocument(Stream stream) => PdfReader.Open(stream, PdfDocumentOpenMode.Import);
    }

    public static Task<PdfDocument[]> ToSections(this IEnumerable<Configuration.Model> configurations, string srcPath) => Resources.Get<ISections>()!.ToSections(configurations, srcPath);
    public static Task<PdfDocument[]> ToSections(this Task<IEnumerable<Configuration.Model>> configurations, string srcPath) => configurations.FlatMap(confs => confs.ToSections(srcPath));
    private static string ToHtml(this string content, string format) => Resources.Get<ISections>()!.ToHtml(content, format);
    private static Task<byte[]> ToPdf(this string html) => Resources.Get<ISections>()!.ToPdf(html);
    public static PdfDocument ToPdfDocument(this Stream stream) => Resources.Get<ISections>()!.ToPdfDocument(stream);
}