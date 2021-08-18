using System.Text.Json;

public interface IJsonService
{
    string Serialize<T>(T obj);
    T Deserialize<T>(string json);
}

public class JsonService : IJsonService
{
    private JsonSerializerOptions options;
    public JsonService(JsonSerializerOptions options) => this.options = options;
    public string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, this.options);
    public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, this.options)!;
}

public static class Json
{
    public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
}