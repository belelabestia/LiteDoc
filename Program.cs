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

var workingPath = args.First();
var liteDoc = new LiteDoc(workingPath);

var configurations = await liteDoc.GetConfigurations();

var sections = await liteDoc.GetSections();

if (sections is null || sections.Any(section => section is null)) throw new Exception("Failed reading sections.");

var parsedSections = configurations.Zip(sections).Select(_ => _.First.Format switch
{
    "html" => _.Second + Environment.NewLine,
    "md" => Markdown.ToHtml(_.Second),
    _ => throw new Exception("Failed parsing sections.")
});

for (int i = 0; i < parsedSections.Count(); i++)
{
    var section = parsedSections.ElementAt(i);
    Directory.CreateDirectory(Path.Combine(workingPath, "dist"));
    await File.WriteAllTextAsync(Path.Combine(workingPath, "dist", $"section{i}.html"), section);
}

for (int i = 0; i < parsedSections.Count(); i++)
{
    using var weasy = new WeasyPrintClient();
    var pdf = await weasy.GeneratePdfFromUrlAsync(Path.Combine(Directory.GetCurrentDirectory(), workingPath, "dist", $"section{i}.html"));
    await File.WriteAllBytesAsync(Path.Combine(workingPath, "dist", $"section{i}.pdf"), pdf);
}

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
