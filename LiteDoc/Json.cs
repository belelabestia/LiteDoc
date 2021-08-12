using System.Text.Json;

public interface IJson
{
    T Deserialize<T>(string json, JsonSerializerOptions options);
}

public static class Json
{
    public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public static IJson Instance = new Default();
    public class Default : IJson
    {
        public T Deserialize<T>(string json, JsonSerializerOptions options) => JsonSerializer.Deserialize<T>(json, options)!;
    }
    public static T Deserialize<T>(this string json, JsonSerializerOptions options) => Instance.Deserialize<T>(json, options);
}