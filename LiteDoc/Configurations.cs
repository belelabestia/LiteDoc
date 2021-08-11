using System.Collections.Generic;
using System.Threading.Tasks;

public interface IConfiguration
{
    Task<IEnumerable<Configuration>> GetConfigurations(string rootPath);
}

public static class Configurations
{
    public const string DefaultFileName = "litedoc.conf.json";
    public static Default Instance = new Default();
    public class Default : IConfiguration
    {
        public Task<IEnumerable<Configuration>> GetConfigurations(string rootPath) => rootPath
            .MovePathTo(DefaultFileName)
            .GetText()
            .Map(json => json.Deserialize<IEnumerable<Configuration>>(Json.DefaultOptions));
    }
    public static Task<IEnumerable<Configuration>> GetConfigurations(this string rootPath) => Instance.GetConfigurations(rootPath);
}
public record Configuration(string Path, string Format);
