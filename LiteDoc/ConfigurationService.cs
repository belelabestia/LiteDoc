using System.Collections.Generic;
using System.Threading.Tasks;

public class ConfigurationService
{
    private string confFileName;
    private JSONService JSONService;
    private IEnumerable<SectionConfiguration>? cachedSections;

    public ConfigurationService(
        string confFileName,
        JSONService JSONService
    )
    {
        this.confFileName = confFileName;
        this.JSONService = JSONService;
    }

    public async Task<IEnumerable<SectionConfiguration>> GetConfigurations() =>
        this.cachedSections ?? await this.JSONService.Deserialize<IEnumerable<SectionConfiguration>>(this.confFileName);
}