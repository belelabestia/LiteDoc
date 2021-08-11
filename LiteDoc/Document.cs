using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;

public interface IDocument
{
    Task WriteDocument(PdfDocument[] sections, string outputPath, string fileName);
    PdfDocument ToDocument(PdfDocument[] sections);
    PdfDocument AddSection(PdfDocument document, PdfDocument section);
    byte[] AsByteArray(PdfDocument document);
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

        public PdfDocument ToDocument(PdfDocument[] sections) => sections.Aggregate(new PdfDocument(), (partial, section) => partial.AddSection(section));

        public PdfDocument AddSection(PdfDocument document, PdfDocument section)
        {
            foreach (var page in section.Pages) document.AddPage(page);
            return document;
        }

        public byte[] AsByteArray(PdfDocument document)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }
    }

    public static Task WriteDocument(this PdfDocument[] sections, string outputPath, string fileName) => Instance.WriteDocument(sections, outputPath, fileName);
    public static Task WriteDocument(this Task<PdfDocument[]> sections, string outputPath, string fileName) => sections.FlatMap(secs => secs.WriteDocument(outputPath, fileName));
    private static PdfDocument ToDocument(this PdfDocument[] sections) => Instance.ToDocument(sections);
    private static PdfDocument AddSection(this PdfDocument document, PdfDocument section) => Instance.AddSection(document, section);
    private static byte[] AsByteArray(this PdfDocument document) => Instance.AsByteArray(document);
}