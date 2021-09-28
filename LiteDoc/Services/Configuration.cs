using System.Collections.Generic;
using System.Threading.Tasks;

using Table = System.Collections.Generic.IEnumerable<System.Collections.Generic.Dictionary<string, string>>;

public interface IConfiguration
{
    Task<Configuration.Model> GetConfiguration(string rootPath);
}

public static class Configuration
{
    public const string DefaultFileName = "litedoc.conf.json";
    public record Section(string Path, string Format);
    public record Replace(Dictionary<string, string> Text, Dictionary<string, Table> Table);
    public record Model(IEnumerable<Section> Sections, Replace Replace);

    public class Service : IConfiguration
    {
        private IFileSystem fileSystem;
        private IJson json;

        public Service(
            IFileSystem fileSystem,
            IJson json
        )
        {
            this.fileSystem = fileSystem;
            this.json = json;
        }

        public Task<Model> GetConfiguration(string rootPath) =>
            rootPath
                .Pipe(rootPath => this.fileSystem.MovePathTo(rootPath, Configuration.DefaultFileName))
                .Pipe(this.fileSystem.GetText)
                .Pipe(this.json.Deserialize<Model>);
    }
}