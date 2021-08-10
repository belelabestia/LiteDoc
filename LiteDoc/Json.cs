using System;
using System.Text.Json;
using System.Threading.Tasks;

public class Json
{
    private WorkSpace workSpace;
    private JsonSerializerOptions options;

    public Json(
        WorkSpace workSpace,
        JsonSerializerOptions options
    )
    {
        this.workSpace = workSpace;
        this.options = options;
    }

    public Json(WorkSpace workplaceService) => this.workSpace = workplaceService;
    public async Task<T> Deserialize<T>(string fileName)
    {
        var json = await this.workSpace.MoveTo(fileName).ReadText() ?? throw new Exception("No json found.");
        return JsonSerializer.Deserialize<T>(json, this.options) ?? throw new Exception("Deserialization returned null.");
    }
}

public static class JsonServiceExtensions
{
    public static Json ToJson(this WorkSpace workSpace) => new Json(workSpace, LiteDocDefault.JsonSerializerOptions);
    public static Json ToJson(this WorkSpace workSpace, JsonSerializerOptions options) => new Json(workSpace, options);
}