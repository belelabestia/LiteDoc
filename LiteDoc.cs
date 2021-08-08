using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Balbarak.WeasyPrint;
using Markdig;
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

public record SectionConfiguration : IValidatable
{
    public string? Path { get; init; }
    public string? Format { get; init; }
    public bool IsValid()
    {
        return this.Path != null && this.Format != null;
    }
}
public class ConfigurationService
{
    private string workingPath;
    public const string ConfFileName = "litedoc.conf.json";

    public ConfigurationService(string workingPath)
    {
        this.workingPath = workingPath;
    }

    public async Task<IEnumerable<SectionConfiguration>> GetConfigurations()
    {
        var deserializationOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var confPath = Path.Combine(this.workingPath, ConfFileName);
        var text = await File.ReadAllTextAsync(confPath);
        var deserialized = JsonSerializer.Deserialize<IEnumerable<SectionConfiguration>>(text, deserializationOptions);
        if (deserialized == null) throw new Exception("Deserialization failed.");
        var safeDeserialized = deserialized.Select(conf => conf.ThrowIfInvalid());
        return safeDeserialized;
    }
}

public class SectionService
{
    private string workingPath;
    private ConfigurationService configurationService;
    public SectionService(
        string workingPath,
        ConfigurationService configurationService
    )
    {
        this.workingPath = workingPath;
        this.configurationService = configurationService;
    }

    public async Task<IEnumerable<string>> GetSections()
    {
        var configurations = await this.configurationService.GetConfigurations();
        var sections = await Task.WhenAll(configurations.Select(conf => File.ReadAllTextAsync(Path.Combine(workingPath, conf.Path!))));
        if (sections is null || sections.Any(section => section is null)) throw new Exception("Failed reading sections.");
        return sections;
    }
}

public class SectionsParser
{
    private ConfigurationService configurationService;
    private SectionService sectionService;

    public SectionsParser(
        ConfigurationService configurationService,
        SectionService sectionService
    )
    {
        this.configurationService = configurationService;
        this.sectionService = sectionService;
    }

    public async Task<IEnumerable<string>> ParseSections()
    {
        var configurations = await this.configurationService.GetConfigurations();
        var sections = await this.sectionService.GetSections();

        return configurations.Zip(sections).Select(_ => _.First.Format switch
        {
            "html" => _.Second + Environment.NewLine,
            "md" => Markdown.ToHtml(_.Second),
            _ => throw new Exception("Failed parsing sections.")
        });
    }
}

public class SectionsWriter
{
    private string workingPath;
    private SectionsParser sectionsParser;
    public SectionsWriter(
        string workingPath,
        SectionsParser sectionsParser
    )
    {
        this.workingPath = workingPath;
        this.sectionsParser = sectionsParser;
    }

    public async Task WriteSections()
    {
        var parsedSections = await this.sectionsParser.ParseSections();

        for (int i = 0; i < parsedSections.Count(); i++)
        {
            var section = parsedSections.ElementAt(i);
            Directory.CreateDirectory(Path.Combine(workingPath, "dist"));
            await File.WriteAllTextAsync(Path.Combine(workingPath, "dist", $"section{i}.html"), section);
        }
    }
}

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