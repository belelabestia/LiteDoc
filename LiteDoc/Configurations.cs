using System.Collections.Generic;
using System.Threading.Tasks;

public interface IConfiguration
{
    Task<IEnumerable<Configuration.Model>> GetConfigurations(string rootPath);
}

public static class Configuration
{
    public const string DefaultFileName = "litedoc.conf.json";
    public class Base : IConfiguration
    {
        public Task<IEnumerable<Model>> GetConfigurations(string rootPath) => rootPath
            .MovePathTo(DefaultFileName)
            .GetText()
            .Map(json => json.Deserialize<IEnumerable<Model>>(Json.DefaultOptions));
    }
    public static Task<IEnumerable<Model>> GetConfigurations(this string rootPath) => Resources.Get<IConfiguration>()!.GetConfigurations(rootPath);
    public record Model(string Path, string Format);

}