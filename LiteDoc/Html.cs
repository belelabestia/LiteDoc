using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class HtmlWritten
{
    public int Count { get; }
    public HtmlWritten(int count) => this.Count = count;
}

public class Html
{
    private WorkSpace workSpace;
    private Parser parser;
    public Html(
        WorkSpace workSpace,
        Parser parser
    )
    {
        this.workSpace = workSpace;
        this.parser = parser;
    }

    public async Task<HtmlWritten> WriteSections()
    {
        this.workSpace.MoveTo("dist").CreateDirectory();
        var sections = await this.parser.ParseSections();
        return await WriteHtmlFromSections(sections);
    }

    private async Task<HtmlWritten> WriteHtmlFromSections(IEnumerable<string> sections)
    {
        var count = sections.Count();
        for (int i = 0; i < count; i++)
        {
            var section = sections.ElementAt(i);
            await this.workSpace.MoveTo($"dist/section{i}.html").Write(section);
        }

        return new HtmlWritten(count);
    }
}

public static class HTMLExtensions
{
    public static Html ToHtml(this (WorkSpace workSpace, Parser parser) deps) => new Html(deps.workSpace, deps.parser);
}