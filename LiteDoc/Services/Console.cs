public interface IConsole
{
    void Print(string text);
}

public static class Console
{
    public class Service : IConsole
    {
        public void Print(string text) => System.Console.WriteLine(text);
    }
}