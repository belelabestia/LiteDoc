using System.Linq;
using System.Threading.Tasks;

public class SectionsWriter
{
    private WorkplaceService workplaceService;
    private SectionsParser sectionsParser;
    public SectionsWriter(
        WorkplaceService workplaceService,
        SectionsParser sectionsParser
    )
    {
        this.workplaceService = workplaceService;
        this.sectionsParser = sectionsParser;
    }

    public async Task WriteSections()
    {
        this.workplaceService.CreateDirectory("dist");
        var parsedSections = await this.sectionsParser.ParseSections();

        for (int i = 0; i < parsedSections.Count(); i++)
        {
            var section = parsedSections.ElementAt(i);
            await this.workplaceService.WriteText("dist/section{i}.html", section);
        }
    }
}
