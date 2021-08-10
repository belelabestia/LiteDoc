using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SectionConfiguration
{
    private string path;
    private string format;
    public string Path => this.path ?? throw new Exception("Invalid null path in configuration.");
    public string Format => this.format ?? throw new Exception("Invalid null format in configuration.");

    public SectionConfiguration(string path, string format)
    {
        this.path = path;
        this.format = format;
    }
}

public class Configuration
{
    private string confFileName;
    private Json json;
    public Configuration(
        Json json,
        string confFileName
    )
    {
        this.confFileName = confFileName;
        this.json = json;
    }

    public Task<IEnumerable<SectionConfiguration>> GetConfigurations() =>
        this.json.Deserialize<IEnumerable<SectionConfiguration>>(this.confFileName);
}

public static class ConfigurationExtensions
{
    public static Configuration ToConfiguration(this Json json) => new Configuration(json, LiteDocDefault.ConfigurationFileName);
    public static Configuration ToConfiguration(this Json json, string confFileName) => new Configuration(json, confFileName);
}