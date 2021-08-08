using System;
using System.Collections.Generic;
using System.Linq;

public interface IOption<T>
{
    T Value { get; }
    IOption<TTo> Select<TTo>(Func<T, TTo> mapper);
    IOption<TTo> Select<TTo>(Func<T, IOption<TTo>> mapper);
}

public interface IValidatable
{
    bool IsValid { get; }
}

public static class Option
{
    public static IOption<T> FromNullable<T>(T? value) =>
        value != null ?
        new Some<T>(value) :
        new None<T>();

    public static IOption<T> FromValidatable<T>(T value) where T : IValidatable =>
        value.IsValid ?
        new Some<T>(value) :
        new None<T>();

    public static IOption<IEnumerable<T>> FromIEnumerable<T>(IEnumerable<IOption<T>> options) =>
        options.Any(option => option.IsNone()) ?
        new None<IEnumerable<T>>() :
        new Some<IEnumerable<T>>(options.Select(option => option.Value));

    public static bool IsSome<T>(IOption<T> option) => option is Some<T>;
    public static bool IsNone<T>(IOption<T> option) => option is None<T>;

    private class Some<T> : IOption<T>
    {
        public T Value { get; init; }

        public Some(T content)
        {
            this.Value = content;
        }

        public IOption<TTo> Select<TTo>(Func<T, TTo> mapper) => new Some<TTo>(mapper(this.Value));

        public IOption<TTo> Select<TTo>(Func<T, IOption<TTo>> mapper) => mapper(this.Value);
    }

    private class None<T> : IOption<T>
    {
        public T Value => throw new Exception("No value available.");

        public IOption<TTo> Select<TTo>(Func<T, TTo> mapper) => new None<TTo>();

        public IOption<TTo> Select<TTo>(Func<T, IOption<TTo>> mapper) => new None<TTo>();
    }
}

public static class OptionExtensions
{
    public static IOption<T> ValidatableToOption<T>(this T value) where T : IValidatable => Option.FromValidatable(value);
    public static IOption<T> NullableToOption<T>(this T? value) => Option.FromNullable(value);
    public static IOption<IEnumerable<T>> IEnumberableToOption<T>(this IEnumerable<IOption<T>> options) => Option.FromIEnumerable(options);
    public static bool IsSome<T>(this IOption<T> option) => Option.IsSome(option);
    public static bool IsNone<T>(this IOption<T> option) => Option.IsNone(option);
}