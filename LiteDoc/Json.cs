using System.Text.Json;

public interface IJsonService
{
    T Deserialize<T>(string json, JsonSerializerOptions options);
}

public class JsonService : IJsonService
{
    public T Deserialize<T>(string json, JsonSerializerOptions options) => JsonSerializer.Deserialize<T>(json, options)!;
}

public static class Json
{
    public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
}