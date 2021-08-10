using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balbarak.WeasyPrint;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public class PdfDocumentWritten { }

public class PdfSectionsWritten
{
    public int Count { get; }
    private WorkSpace workSpace;
    public PdfSectionsWritten(int count, WorkSpace workSpace)
    {
        this.Count = count;
        this.workSpace = workSpace;
    }
    public PdfDocumentWritten WriteDocument()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var final = new PdfDocument();

        for (int i = 0; i < this.Count; i++)
        {
            var document = PdfReader.Open(this.workSpace.MoveTo($"dist/section{i}.pdf").WorkingPath, PdfDocumentOpenMode.Import);
            foreach (var page in document.Pages)
            {
                final.AddPage(page);
            }
        }

        final.Save(this.workSpace.MoveTo("dist/output.pdf").WorkingPath);

        return new PdfDocumentWritten();
    }
}

public static class PdfSectionsWrittenExtensions
{
    public static PdfSectionsWritten ToPdfSectionsWritten(this (int count, WorkSpace workSpace) deps) => new PdfSectionsWritten(deps.count, deps.workSpace);
}

public class PdfWriter
{
    private Configuration configuration;
    private WorkSpace workSpace;
    private Html html;
    public PdfWriter(
        Configuration configuration,
        WorkSpace workSpace,
        Html html
    )
    {
        this.configuration = configuration;
        this.workSpace = workSpace;
        this.html = html;
    }

    public async Task<PdfDocumentWritten> WriteDocument() => (await this.WriteSections()).WriteDocument();

    public async Task<PdfSectionsWritten> WriteSections()
    {
        var result = await this.html.WriteSections();

        for (int i = 0; i < result.Count; i++)
        {
            using var weasy = new WeasyPrintClient();
            var html = await this.workSpace.MoveTo($"dist/section{i}.html").ReadText();
            var bytes = await weasy.GeneratePdfAsync(html);
            await this.workSpace.MoveTo($"dist/section{i}.pdf").Write(bytes);
        }

        return (result.Count, this.workSpace).ToPdfSectionsWritten();
    }
}

public static class PdfWriterExtensions
{
    public static PdfWriter ToPdfWriter(this (Configuration configuration, WorkSpace workSpace, Html html) deps) => new PdfWriter(deps.configuration, deps.workSpace, deps.html);
}