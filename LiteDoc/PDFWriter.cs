using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balbarak.WeasyPrint;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public class PDFWriter
{
    private ConfigurationService configurationService;
    private WorkplaceService workplaceService;
    private SectionsWriter sectionsWriter;
    public PDFWriter(
        ConfigurationService configurationService,
        WorkplaceService workplaceService,
        SectionsWriter sectionsWriter
    )
    {
        this.configurationService = configurationService;
        this.workplaceService = workplaceService;
        this.sectionsWriter = sectionsWriter;
    }

    public async Task WriteDocument()
    {
        await this.WriteSections();
        var parsedSections = await this.configurationService.GetConfigurations();

        var final = new PdfDocument();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        for (int i = 0; i < parsedSections.Count(); i++)
        {
            var document = PdfReader.Open(this.workplaceService.GetPathOf($"dist/section{i}.pdf"), PdfDocumentOpenMode.Import);
            foreach (var page in document.Pages)
            {
                final.AddPage(page);
            }
        }

        final.Save(this.workplaceService.GetPathOf("dist/output.pdf"));
    }

    public async Task WriteSections()
    {
        await this.sectionsWriter.WriteSections();
        var sectionsCount = (await this.configurationService.GetConfigurations()).Count();

        for (int i = 0; i < sectionsCount; i++)
        {
            using var weasy = new WeasyPrintClient();
            var html = await this.workplaceService.ReadText($"dist/section{i}.html");
            var bytes = await weasy.GeneratePdfAsync(html);
            await this.workplaceService.WriteBytes($"dist/section{i}.pdf", bytes);
        }
    }
}