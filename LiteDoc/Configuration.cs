using System.Collections.Generic;
using System.Threading.Tasks;

public interface IConfigurationService
{
    Task<IEnumerable<Configuration>> GetConfigurations(string rootPath);
}

public class ConfigurationService : IConfigurationService
{
    // conf.create
    // conf.update
    // conf.addSection
    // conf.removeSection
    private IFileSystemService fileSystem;
    private IJsonService json;

    public ConfigurationService(IFileSystemService fileSystem, IJsonService json)
    {
        this.fileSystem = fileSystem;
        this.json = json;
    }

    public async Task<IEnumerable<Configuration>> GetConfigurations(string rootPath)
    {
        var filePath = this.fileSystem.MovePathTo(rootPath, Configuration.DefaultFileName);
        var text = await this.fileSystem.GetText(filePath);
        var configurations = this.json.Deserialize<IEnumerable<Configuration>>(text);
        return configurations;
    }
}

public record Configuration(string Path, string Format)
{
    public const string DefaultFileName = "litedoc.conf.json";
}