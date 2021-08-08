using System.Text.Json;
using System.Threading.Tasks;

public class JSONService
{
    private WorkplaceService workplaceService;
    private JsonSerializerOptions jsonSerializerOptions;

    public JSONService(
        WorkplaceService workplaceService,
        JsonSerializerOptions jsonSerializerOptions
    )
    {
        this.workplaceService = workplaceService;
        this.jsonSerializerOptions = jsonSerializerOptions;
    }
    public async Task<T?> Deserialize<T>(string fileName)
    {
        var json = await this.workplaceService.ReadTextOf(fileName);
        return JsonSerializer.Deserialize<T>(json, this.jsonSerializerOptions);
    }
}