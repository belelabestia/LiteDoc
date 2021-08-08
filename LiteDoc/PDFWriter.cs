using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Balbarak.WeasyPrint;

public class PDFWriter
{
    private string workingPath;
    private SectionsParser sectionsParser;
    private SectionsWriter sectionsWriter;
    public PDFWriter(
        string workingPath,
        SectionsParser sectionsParser,
        SectionsWriter sectionsWriter
    )
    {
        this.workingPath = workingPath;
        this.sectionsParser = sectionsParser;
        this.sectionsWriter = sectionsWriter;
    }
    public async Task WritePDFs()
    {
        await this.sectionsWriter.WriteSections();

        var parsedSections = await this.sectionsParser.ParseSections();

        for (int i = 0; i < parsedSections.Count(); i++)
        {
            using var weasy = new WeasyPrintClient();
            var inputPathFile = Path.Combine(workingPath, "dist", $"section{i}.html");
            var outputPathFile = Path.Combine(workingPath, "dist", $"section{i}.pdf");

            var html = await File.ReadAllTextAsync(inputPathFile);
            var bytes = await weasy.GeneratePdfAsync(html);
            await File.WriteAllBytesAsync(outputPathFile, bytes);
        }
    }
}