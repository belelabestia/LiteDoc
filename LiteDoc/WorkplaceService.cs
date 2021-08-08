using System.IO;
using System.Threading.Tasks;

public class WorkplaceService
{
    public string WorkingPath { get; init; }

    public WorkplaceService(string workingPath)
    {
        this.WorkingPath = workingPath;
    }

    public Task<string> ReadTextOf(string fileName)
    {
        var path = this.GetPathOf(fileName);
        return File.ReadAllTextAsync(path);
    }

    public string GetPathOf(string fileName)
    {
        return Path.Combine(this.WorkingPath, fileName);
    }
}