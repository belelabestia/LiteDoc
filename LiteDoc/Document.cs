using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PdfSharp.Pdf;

public interface IDocument
{
    Task WriteDocument(PdfDocument[] sections, string outputFileName);
    Task WriteDocument(Task<PdfDocument[]> sections, string outputFileName);
    PdfDocument ToDocument(PdfDocument[] sections);
    PdfDocument AddSection(PdfDocument document, PdfDocument section);
    byte[] AsByteArray(PdfDocument document);
}

public static class Document
{
    public static Default Instance = new Default();
    public class Default : IDocument
    {
        public Task WriteDocument(PdfDocument[] sections, string outputFileName) => sections
            .ToDocument()
            .AsByteArray()
            .Save(outputFileName);

        public Task WriteDocument(Task<PdfDocument[]> sections, string outputFileName) => sections.FlatMap(secs => secs.WriteDocument(outputFileName));
        public PdfDocument ToDocument(PdfDocument[] sections) => sections.Aggregate(new PdfDocument(), (partial, section) => partial.AddSection(section));

        public PdfDocument AddSection(PdfDocument document, PdfDocument section)
        {
            foreach (var page in section.Pages) document.AddPage(page);
            return document;
        }

        public byte[] AsByteArray(PdfDocument document)
        {
            var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }
    }

    public static Task WriteDocument(this PdfDocument[] sections, string outputFileName) => Instance.WriteDocument(sections, outputFileName);
    public static Task WriteDocument(this Task<PdfDocument[]> sections, string outputFileName) => sections.FlatMap(secs => secs.WriteDocument(outputFileName));
    private static PdfDocument ToDocument(this PdfDocument[] sections) => Instance.ToDocument(sections);
    private static PdfDocument AddSection(this PdfDocument document, PdfDocument section) => Instance.AddSection(document, section);
    private static byte[] AsByteArray(this PdfDocument document)=>Instance.AsByteArray(document);
}