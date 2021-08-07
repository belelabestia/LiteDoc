using System;

public interface IValidatable
{
    bool IsValid();
}

public static class IsValidExtensions
{
    public static T ThrowIfInvalid<T>(this T validatable) where T : IValidatable
    {
        if (!validatable.IsValid()) throw new Exception("Something's invalid.");
        return validatable;
    }

    public static T? ValidOrDefault<T>(this T validatable) where T : IValidatable
    {
        return validatable.IsValid() ? validatable : default(T);
    }
}