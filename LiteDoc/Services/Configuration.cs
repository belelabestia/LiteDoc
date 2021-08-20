using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IConfiguration
{
    Task<IEnumerable<Configuration.Model>> GetConfiguration(string rootPath);
}

public static class Configuration
{
    public const string DefaultFileName = "litedoc.conf.json";
    public record Model(string Path, string Format);
    public class Service : IConfiguration
    {
        private IFileSystem fileSystem;
        private IJson json;

        public Service(IFileSystem fileSystem, IJson json)
        {
            this.fileSystem = fileSystem;
            this.json = json;
        }

        public Task<IEnumerable<Model>> GetConfiguration(string rootPath) =>
            rootPath
                .Pipe(this.MovePathTo(Configuration.DefaultFileName))
                .Pipe(this.fileSystem.GetText)
                .Pipe(this.json.Deserialize<IEnumerable<Model>>);

        private Func<string, string> MovePathTo(string fileName) =>
            rootPath => this.fileSystem.MovePathTo(rootPath, fileName);
    }
}