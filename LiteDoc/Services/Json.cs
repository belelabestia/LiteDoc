using System.Text.Json;

public interface IJson
{
    string Serialize<T>(T obj);
    T Deserialize<T>(string json);
}

public static class Json
{
    public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public class Service : IJson
    {
        private JsonSerializerOptions options;
        public Service(JsonSerializerOptions options) => this.options = options;
        public string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, this.options);
        public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, this.options)!;
    }
}