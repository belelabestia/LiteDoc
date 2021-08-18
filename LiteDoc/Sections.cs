using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Balbarak.WeasyPrint;
using Markdig;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public interface ISectionService
{
    string ToHtml(string content, string format);
    Task<byte[]> ToPdf(string html);
    Task<PdfDocument[]> ToSections(IEnumerable<Configuration> configurations, string srcPath);
    PdfDocument ToPdfDocument(Stream stream);
}

public class SectionService : ISectionService
{
    private IFileSystemService fileSystemService;
    public SectionService(IFileSystemService fileSystemService) => this.fileSystemService = fileSystemService;

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

    public async Task<PdfDocument[]> ToSections(IEnumerable<Configuration> configurations, string srcPath)
    {
        var tasks = configurations
            .AsParallel()
            .AsOrdered()
            .Select(async configuration =>
            {
                var path = this.fileSystemService.MovePathTo(srcPath, configuration.Path);
                var text = await this.fileSystemService.GetText(path);
                var html = this.ToHtml(text, configuration.Format);
                var pdf = await this.ToPdf(html);
                var stream = this.fileSystemService.ToMemoryStream(pdf);
                var section = this.ToPdfDocument(stream);
                return section;
            });

        var sections = await Task.WhenAll(tasks);
        return sections;
    }
}