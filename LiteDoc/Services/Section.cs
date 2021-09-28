using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Balbarak.WeasyPrint;
using Markdig;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public interface ISection
{
    Task<PdfDocument[]> ToSections(Configuration.Model configuration, string srcPath);
}

public static class Section
{

    public class Service : ISection
    {
        private IFileSystem fileSystem;
        private IParser parser;

        public Service(IFileSystem fileSystem, IParser parser)
        {
            this.fileSystem = fileSystem;
            this.parser = parser;
        }

        public Task<PdfDocument[]> ToSections(Configuration.Model configuration, string srcPath) =>
            configuration.Sections
                .AsParallel()
                .AsOrdered()
                .Select(section =>
                    this.fileSystem.MovePathTo(srcPath, section.Path)
                        .Pipe(this.fileSystem.GetText)
                        .Pipe(this.ToHtml(section.Format))
                        .With(this.GetStyle(configuration, srcPath))
                        .Pipe(this.PrependStyleToHtml)
                        .Pipe(html => this.parser.Parse(html, configuration))
                        .Pipe(this.ToPdf)
                        .Pipe(this.fileSystem.ToMemoryStream)
                        .Pipe(this.ToPdfDocument)
                )
                .Pipe(Task.WhenAll);

        private Func<string, string> ToHtml(string format) =>
            text => this.ToHtml(text, format);

        private string ToHtml(string content, string format) => format switch
        {
            "html" => content + Environment.NewLine,
            "md" => Markdown.ToHtml(content),
            _ => throw new Exception("Invalid format.")
        };

        private string PrependStyleToHtml(string html, string style) => $"<style>{style}</style>{html}";

        private Task<string> GetStyle(Configuration.Model configuration, string srcPath) =>
            srcPath
                .Pipe(path => this.fileSystem.MovePathTo(path, configuration.Style))
                .Pipe(this.fileSystem.GetText);

        private Task<byte[]> ToPdf(string html)
        {
            using var weasy = new WeasyPrintClient();
            return weasy.GeneratePdfAsync(html);
        }

        private PdfDocument ToPdfDocument(Stream stream) => PdfReader.Open(stream, PdfDocumentOpenMode.Import);
    }
}