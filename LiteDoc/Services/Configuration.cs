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
        private IConsole console;

        public Service(
            IFileSystem fileSystem,
            IJson json,
            IConsole console
        )
        {
            this.fileSystem = fileSystem;
            this.json = json;
            this.console = console;
        }

        public Task<IEnumerable<Model>> GetConfiguration(string rootPath) =>
            rootPath
                .Pipe(rootPath => this.fileSystem.MovePathTo(rootPath, Configuration.DefaultFileName))
                .Effect(confFile => this.console.Print($"Fetching configuration from file {confFile}."))
                .Pipe(this.fileSystem.GetText)
                .Effect(() => this.console.Print("Deserializing configuration..."))
                .Pipe(this.json.Deserialize<IEnumerable<Model>>)
                .Effect(() => this.console.Print("Successfully deserialized configuration."));
    }
}