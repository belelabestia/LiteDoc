using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public class LiteDoc
{
    private string workingPath;
    private SectionsParser sectionsParser;
    private PDFWriter PDFWriter;

    public LiteDoc(
        string workingPath,
        SectionsParser sectionsParser,
        PDFWriter PDFWriter
    )
    {
        this.workingPath = workingPath;
        this.sectionsParser = sectionsParser;
        this.PDFWriter = PDFWriter;
    }

    public async Task WriteDocument()
    {
        await this.PDFWriter.WritePDFs();
        var parsedSections = await this.sectionsParser.ParseSections();

        var final = new PdfDocument();

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        for (int i = 0; i < parsedSections.Count(); i++)
        {
            var document = PdfReader.Open(Path.Combine(workingPath, "dist", $"section{i}.pdf"), PdfDocumentOpenMode.Import);
            foreach (var page in document.Pages)
            {
                final.AddPage(page);
            }
        }

        final.Save(Path.Combine(workingPath, "dist", "output.pdf"));
    }
}
