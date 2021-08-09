using System.Threading.Tasks;

public class LiteDoc
{
    private PDFWriter PDFWriter;
    public LiteDoc(PDFWriter PDFWriter) => this.PDFWriter = PDFWriter;
    public Task StartWatching() => this.PDFWriter.WriteDocument();
}
