using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;

public class Parser
{
    private Configuration configuration;
    private Section section;

    public Parser(
        Configuration configuration,
        Section section
    )
    {
        this.configuration = configuration;
        this.section = section;
    }

    public async Task<IEnumerable<string>> ParseSections()
    {
        var configurations = await this.configuration.GetConfigurations();
        var sections = await this.section.GetSections();

        return configurations.Zip(sections).Select(_ => _.First.Format switch
        {
            "html" => _.Second + Environment.NewLine,
            "md" => Markdown.ToHtml(_.Second),
            _ => throw new Exception("Failed parsing sections.")
        });
    }
}

public static class ParserExtensions
{
    public static Parser ToParser(this (Configuration configuration, Section section) deps) => new Parser(deps.configuration, deps.section);
}