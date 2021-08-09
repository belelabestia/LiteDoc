using System.Text;

public class PDFService
{
    private WorkplaceService workplaceService;
    public PDFService(
        WorkplaceService workplaceService
    )
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        this.workplaceService = workplaceService;
    }

    
}