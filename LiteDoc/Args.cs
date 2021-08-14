public interface IArgsService
{
    string Command { get; }
    string Path { get; }
}

public class ArgsService : IArgsService
{
    private string[] args;

    public ArgsService(string[] args) => this.args = args;

    public string Command => args[0];
    public string Path => args[1];
}
