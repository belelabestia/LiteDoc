using System;
using System.IO;
using System.Linq;
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

        private Func<byte[], Task> Save(string outputPath, string fileName) =>
            bytes => this.fileSystem.Save(bytes, outputPath, fileName);
    }
}