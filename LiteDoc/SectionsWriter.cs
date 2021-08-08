using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class SectionsWriter
{
    private string workingPath;
    private SectionsParser sectionsParser;
    public SectionsWriter(
        string workingPath,
        SectionsParser sectionsParser
    )
    {
        this.workingPath = workingPath;
        this.sectionsParser = sectionsParser;
    }

    public async Task WriteSections()
    {
        var parsedSections = await this.sectionsParser.ParseSections();

        for (int i = 0; i < parsedSections.Count(); i++)
        {
            var section = parsedSections.ElementAt(i);
            Directory.CreateDirectory(Path.Combine(workingPath, "dist"));
            await File.WriteAllTextAsync(Path.Combine(workingPath, "dist", $"section{i}.html"), section);
        }
    }
}
