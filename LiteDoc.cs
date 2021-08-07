using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class LiteDoc
{
    public const string ConfFileName = "litedoc.conf.json";
    private string workingPath;
    public string confPath => Path.Combine(this.workingPath, ConfFileName);
    public Task<string> confAsTextAsync => File.ReadAllTextAsync(this.confPath);
    public LiteDoc(string workingPath)
    {
        this.workingPath = workingPath;
    }

    public async Task<IEnumerable<SectionConfiguration>> GetConfigurations()
    {
        var deserializationOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var text = await confAsTextAsync;
        var deserialized = JsonSerializer.Deserialize<IEnumerable<SectionConfiguration>>(text, deserializationOptions);
        if (deserialized == null) throw new Exception("Deserialization failed.");
        var safeDeserialized = deserialized.Select(conf => conf.ThrowIfInvalid());
        return safeDeserialized;
    }

    public async Task<IEnumerable<string>> GetSections()
    {
        var configurations = await this.GetConfigurations();
        return await Task.WhenAll(configurations.Select(conf => File.ReadAllTextAsync(Path.Combine(workingPath, conf.Path!))));
    }
}