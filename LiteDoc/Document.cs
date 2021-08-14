using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;

public interface IDocumentService
{
    Task WriteDocument(PdfDocument[] sections, string outputPath, string fileName);
}

public class DocumentService : IDocumentService
{
    private IFileSystemService fileSystem;
    public DocumentService(IFileSystemService fileSystem) => this.fileSystem = fileSystem;

    public Task WriteDocument(PdfDocument[] sections, string outputPath, string fileName)
    {
        var document = sections.Aggregate(new PdfDocument(), (partial, section) => this.AddSection(partial, section));
        var bytes = this.AsByteArray(document);
        return this.fileSystem.Save(bytes, outputPath, fileName);
    }

    private PdfDocument AddSection(PdfDocument document, PdfDocument section)
    {
        foreach (var page in section.Pages) document.AddPage(page);
        return document;
    }

    private byte[] AsByteArray(PdfDocument document)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }
}