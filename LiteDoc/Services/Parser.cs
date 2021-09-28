using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Table = System.Collections.Generic.IEnumerable<System.Collections.Generic.Dictionary<string, string>>;

public interface IParser
{
    string Parse(string html, Configuration.Model configuration);
}

public static class Parser
{
    public class Service : IParser
    {
        public string Parse(string html, Configuration.Model configuration) => Regex.Replace(html, @"{(.+?):(\S.+?)}", GetReplacer(configuration));

        private MatchEvaluator GetReplacer(Configuration.Model configuration) => (Match match) =>
            match
                .Pipe(match => (match.Groups[1].Value, match.Groups[2].Value))
                .Pipe<string, string, string>((string type, string key) => type switch
                {
                    "text" => configuration.Replace.Text[key],
                    "table" => configuration.Replace.Table[key].Pipe(this.ToTableString),
                    _ => throw new Exception("Unsupported replacement type.")
                });

        private string ToTableString(Table table) =>
            this.GetHeadings(table)
                .With(this.GetTableRows(table))
                .Pipe((headings, rows) =>
                (
                    headings
                        .Select(heading => $"<th>{heading}</th>")
                        .Pipe(string.Concat),
                    rows
                        .Select(row => $"<tr>{row}</tr>")
                        .Pipe(string.Concat))
                )
                .Pipe((ths, trs) => $"<table><tr>{ths}</tr>{trs}</table>");

        private IEnumerable<string> GetHeadings(Table table) =>
            table
                .SelectMany(record => record.Keys)
                .Distinct();

        private IEnumerable<string> GetTableRows(Table table) =>
            table
                .Select(row => this.GetHeadings(table)
                    .Select(heading => $"<td>{row.GetValueOrDefault(heading) ?? ""}</td>")
                    .Pipe(string.Concat)
                );
    }
}