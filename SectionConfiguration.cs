public record SectionConfiguration : IValidatable
{
    public string? Path { get; init; }
    public string? Format { get; init; }
    public bool IsValid()
    {
        return this.Path != null && this.Format != null;
    }
}
