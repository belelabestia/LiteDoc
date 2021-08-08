using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class SectionService
{
    private ConfigurationService configurationService;
    private WorkplaceService workplaceService;
    public SectionService(
        ConfigurationService configurationService,
        WorkplaceService workplaceService
    )
    {
        this.configurationService = configurationService;
        this.workplaceService = workplaceService;
    }

    public async Task<IEnumerable<string>> GetSections()
    {
        var configurations = await this.configurationService.GetConfigurations();
        return await Task.WhenAll(configurations.Select(conf => this.workplaceService.ReadText(conf.Path)));
    }
}