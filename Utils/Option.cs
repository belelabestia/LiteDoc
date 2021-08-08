using System;
using System.Collections.Generic;
using System.Linq;

public interface IValidatable
{
    bool IsValid { get; }
}
public interface IOption<T> { }
public class None<T> : IOption<T> { }

public class Some<T> : IOption<T>
{
    public T Value { get; init; }
    public Some(T content) => this.Value = content;
}

public static class Option
{
    public static IOption<T> From<T>(T? value) =>
        value != null ?
            new Some<T>(value) :
            new None<T>();

    public static IOption<T> ToOption<T>(this T value) where T : IValidatable =>
        value.IsValid ?
            new Some<T>(value) :
            new None<T>();

    public static IOption<IEnumerable<T>> ToOption<T>(this IEnumerable<IOption<T>> options) =>
        options.Any(option => option is None<T>) ?
            new None<IEnumerable<T>>() :
            new Some<IEnumerable<T>>(options.Select(option => ((Some<T>)option).Value));

    public static IOption<TTo> Select<T, TTo>(this IOption<T> option, Func<T, TTo> mapper) =>
        option is None<T> ?
            new None<TTo>() :
            new Some<TTo>(mapper(((Some<T>)option).Value));

    public static IOption<TTo> Select<T, TTo>(this IOption<T> option, Func<T, IOption<TTo>> mapper) =>
        option is None<T> ?
            new None<TTo>() :
            mapper(((Some<T>)option).Value);

    public static T Reveal<T>(this IOption<T> option, string errorMessage = "Sorry, nothing to reveal.") =>
        option is None<T> ?
            throw new Exception(errorMessage) :
            ((Some<T>)option).Value;
}