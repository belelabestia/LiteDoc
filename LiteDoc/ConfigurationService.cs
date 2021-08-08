using System.Collections.Generic;
using System.Threading.Tasks;

public class ConfigurationService
{
    private string confFileName;
    private JSONService JSONService;

    public ConfigurationService(
        string confFileName,
        JSONService JSONService
    )
    {
        this.confFileName = confFileName;
        this.JSONService = JSONService;
    }

    public Task<IEnumerable<SectionConfiguration>> GetConfigurations() =>
        this.JSONService.Deserialize<IEnumerable<SectionConfiguration>>(this.confFileName);
}