using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;

public interface IDocument
{
    Task WriteDocument(PdfDocument[] sections, string outputPath, string fileName);
}

public static class Document
{
    public static Default Instance = new Default();
    public class Default : IDocument
    {
        public Task WriteDocument(PdfDocument[] sections, string outputPath, string fileName) => sections
            .ToDocument()
            .AsByteArray()
            .Save(outputPath, fileName);
    }

    public static Task WriteDocument(this Task<PdfDocument[]> sections, string outputPath, string fileName) => sections.FlatMap(secs => secs.WriteDocument(outputPath, fileName));
    private static Task WriteDocument(this PdfDocument[] sections, string outputPath, string fileName) => Instance.WriteDocument(sections, outputPath, fileName);
    private static PdfDocument ToDocument(this PdfDocument[] sections) => sections.Aggregate(new PdfDocument(), (partial, section) => partial.AddSection(section));

    private static PdfDocument AddSection(this PdfDocument document, PdfDocument section)
    {
        foreach (var page in section.Pages) document.AddPage(page);
        return document;
    }

    private static byte[] AsByteArray(this PdfDocument document)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }
}