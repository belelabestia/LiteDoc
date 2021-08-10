using System.Text;

public class PDFService
{
    private WorkSpace workplaceService;
    public PDFService(
        WorkSpace workplaceService
    )
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        this.workplaceService = workplaceService;
    }

    
}