using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class SectionService
{
    private string workingPath;
    private ConfigurationService configurationService;
    public SectionService(
        string workingPath,
        ConfigurationService configurationService
    )
    {
        this.workingPath = workingPath;
        this.configurationService = configurationService;
    }

    public async Task<IEnumerable<string>> GetSections()
    {
        var configurations = await this.configurationService.GetConfigurations();
        var sections = await Task.WhenAll(configurations.Select(conf => File.ReadAllTextAsync(Path.Combine(workingPath, conf.Path!))));
        if (sections is null || sections.Any(section => section is null)) throw new Exception("Failed reading sections.");
        return sections;
    }
}
