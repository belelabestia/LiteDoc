using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Balbarak.WeasyPrint;
using Markdig;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public interface ISection
{
    string ToHtml(string content, string format);
    Task<byte[]> ToPdf(string html);
    Task<PdfDocument[]> ToSections(IEnumerable<Configuration.Model> configurations, string srcPath);
    PdfDocument ToPdfDocument(Stream stream);
}

public static class Section
{
    public class Service : ISection
    {
        private IFileSystem fileSystem;
        public Service(IFileSystem fileSystem) => this.fileSystem = fileSystem;

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

        public Task<PdfDocument[]> ToSections(IEnumerable<Configuration.Model> configurations, string srcPath) =>
            configurations
                .AsParallel()
                .AsOrdered()
                .Select(configuration =>
                    this.fileSystem.MovePathTo(srcPath, configuration.Path)
                        .Pipe(this.fileSystem.GetText)
                        .Pipe(this.ToHtml(configuration.Format))
                        .Pipe(this.ToPdf)
                        .Pipe(this.fileSystem.ToMemoryStream)
                        .Pipe(this.ToPdfDocument)
                )
                .Pipe(Task.WhenAll);

        private Func<string, string> ToHtml(string format) =>
            text => this.ToHtml(text, format);
    }
}