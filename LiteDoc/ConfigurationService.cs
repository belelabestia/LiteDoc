using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<IEnumerable<SectionConfiguration>> GetConfigurations() =>
        (await this.JSONService.Deserialize<IEnumerable<SectionConfiguration>>(this.confFileName))
            .NullableToOption()
            .Select(conf => conf.Select(section => section.ValidatableToOption()))
            .Select(options => options.IEnumberableToOption())
            .Value;
}