public record SectionConfiguration : IValidatable
{
    public virtual string? Path { get; init; }
    public virtual string? Format { get; init; }

    public bool IsValid => this.Path != null && this.Format != null;
}
