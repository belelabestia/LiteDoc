using System.Linq;
using System.Text.RegularExpressions;

public interface IParser
{
    string Parse(string html, Configuration.Model configuration);
}

public static class Parser
{
    public class Service : IParser
    {
        public string Parse(string html, Configuration.Model configuration) => Regex.Replace(html, "{%(.+?)%}", GetReplacer(configuration));

        private MatchEvaluator GetReplacer(Configuration.Model configuration) => (Match match) =>
            match
                .Pipe(match => match.Groups[1].Value)
                .Pipe(key => configuration.Replace.Text[key]);
    }
}