using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Section
{
    private Configuration configuration;
    private WorkSpace workSpace;
    public Section(
        Configuration configuration,
        WorkSpace workSpace
    )
    {
        this.configuration = configuration;
        this.workSpace = workSpace;
    }

    public Task<string[]> GetSections() => (this.configuration.GetConfigurations(), this.workSpace).ToSections();
}

public static class SectionExtensions
{
    public static Section ToSection(this (Configuration configuration, WorkSpace workSpace) deps) => new Section(deps.configuration, deps.workSpace);
    public static async Task<string[]> ToSections(this (Task<IEnumerable<SectionConfiguration>> configurations, WorkSpace workSpace) deps) => await Task.WhenAll((await deps.configurations).Select(conf => deps.workSpace.MoveTo(conf.Path).ReadText()));
}