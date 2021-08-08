using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public class LiteDoc
{
    private WorkplaceService workplaceService;
    private SectionsParser sectionsParser;
    private PDFWriter PDFWriter;

    public LiteDoc(
        WorkplaceService workplaceService,
        SectionsParser sectionsParser,
        PDFWriter PDFWriter
    )
    {
        this.workplaceService = workplaceService;
        this.sectionsParser = sectionsParser;
        this.PDFWriter = PDFWriter;
    }
}