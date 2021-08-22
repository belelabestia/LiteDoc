using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;

public interface IDocument
{
    Task WriteDocument(PdfDocument[] sections, string outputPath, string fileName);
}

public static class Document
{
    public class Service : IDocument
    {
        private IFileSystem fileSystem;
        public Service(IFileSystem fileSystem) => this.fileSystem = fileSystem;

        public Task WriteDocument(PdfDocument[] sections, string outputPath, string fileName) =>
            sections
                .Aggregate(new PdfDocument(), this.AddSection)
                .Pipe(this.AsByteArray)
                .Pipe(this.Save(outputPath, fileName));

        private PdfDocument AddSection(PdfDocument document, PdfDocument section) =>
            section.Pages
                .Pipe(pages => (IEnumerable<PdfPage>)pages)
                .Aggregate(document, this.AddPage);

        private byte[] AsByteArray(PdfDocument document) =>
            new MemoryStream()
                .Effect(() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance))
                .Effect(stream => document.Save(stream, false))
                .ToArray();

        private Func<byte[], Task> Save(string outputPath, string fileName) =>
            bytes => this.fileSystem.Save(bytes, outputPath, fileName);

        private PdfDocument AddPage(PdfDocument document, PdfPage page) =>
            document.Effect(() => document.AddPage(page));
    }
}