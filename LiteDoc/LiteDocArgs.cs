public interface ICommand { string Command { get; } }
public interface IPath { string WorkspacePath { get; } }
public record LiteDocArgs(string Command, string WorkspacePath) : ICommand, IPath;
