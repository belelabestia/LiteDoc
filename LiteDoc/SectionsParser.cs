using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;

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
