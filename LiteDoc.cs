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
    public const string ConfFileName = "litedoc.conf.json";
    private string workingPath;
    public string confPath => Path.Combine(this.workingPath, ConfFileName);
    public Task<string> confAsTextAsync => File.ReadAllTextAsync(this.confPath);
    public LiteDoc(string workingPath)
    {
        this.workingPath = workingPath;
    }

    public async Task<IEnumerable<SectionConfiguration>> GetConfigurations()
    {
        var deserializationOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var text = await confAsTextAsync;
        var deserialized = JsonSerializer.Deserialize<IEnumerable<SectionConfiguration>>(text, deserializationOptions);
        if (deserialized == null) throw new Exception("Deserialization failed.");
        var safeDeserialized = deserialized.Select(conf => conf.ThrowIfInvalid());
        return safeDeserialized;
    }

    public async Task<IEnumerable<string>> GetSections()
    {
        var configurations = await this.GetConfigurations();
        var sections = await Task.WhenAll(configurations.Select(conf => File.ReadAllTextAsync(Path.Combine(workingPath, conf.Path!))));
        if (sections is null || sections.Any(section => section is null)) throw new Exception("Failed reading sections.");
        return sections;
    }

    public async Task<IEnumerable<string>> GetParsedSections()
    {
        var configurations = await this.GetConfigurations();
        var sections = await this.GetSections();

        return configurations.Zip(sections).Select(_ => _.First.Format switch
        {
            "html" => _.Second + Environment.NewLine,
            "md" => Markdown.ToHtml(_.Second),
            _ => throw new Exception("Failed parsing sections.")
        });
    }

    public async Task WriteSections()
    {
        var parsedSections = await this.GetParsedSections();

        for (int i = 0; i < parsedSections.Count(); i++)
        {
            var section = parsedSections.ElementAt(i);
            Directory.CreateDirectory(Path.Combine(workingPath, "dist"));
            await File.WriteAllTextAsync(Path.Combine(workingPath, "dist", $"section{i}.html"), section);
        }
    }

    public async Task WritePDFs()
    {
        await this.WriteSections();

        var parsedSections = await this.GetParsedSections();

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

    public async Task WriteDocument()
    {
        await this.WritePDFs();
        var parsedSections = await this.GetParsedSections();

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