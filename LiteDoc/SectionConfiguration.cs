using System;

public record SectionConfiguration
{
    private string path;
    private string format;
    public string Path => this.path ?? throw new Exception("Invalid null path in configuration.");
    public string Format => this.format ?? throw new Exception("Invalid null format in configuration.");

    public SectionConfiguration(string path, string format)
    {
        this.path = path;
        this.format = format;
    }
}
