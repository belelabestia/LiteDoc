using System;
using System.Threading.Tasks;

public static class Pipes
{
    public static B Pipe<A, B>(this A a, Func<A, B> func) => func(a);
    public static async Task<B> Pipe<A, B>(this Task<A> a, Func<A, B> func) => (await a).Pipe(func);
    public static async Task<B> Pipe<A, B>(this Task<A> a, Func<A, Task<B>> func) => await (await a).Pipe(func);
    public static C Pipe<A, B, C>(this (A, B) ab, Func<A, B, C> func) => func(ab.Item1, ab.Item2);
    public static async Task<C> Pipe<A, B, C>(this Task<(A, B)> ab, Func<A, B, C> func) => (await ab).Pipe(func);
    public static async Task<C> Pipe<A, B, C>(this Task<(A, B)> ab, Func<A, B, Task<C>> func) => await (await ab).Pipe(func);
}

public static class Tuples
{
    public static (A, B) With<A, B>(this A a, B b) => (a, b);
    public static async Task<(A, B)> With<A, B>(this Task<A> a, B b) => (await a, b);
    public static async Task<(A, B)> With<A, B>(this A a, Task<B> b) => (a, await b);
    public static async Task<(A, B)> With<A, B>(this Task<A> a, Task<B> b) => (await a, await b);

}

public static class Effects
{
    public static A Effect<A>(this A a, Action action)
    {
        action();
        return a;
    }

    public static A Effect<A>(this A a, Action<A> action)
    {
        action(a);
        return a;
    }

    public static async Task<A> Effect<A>(this A a, Func<Task> action)
    {
        await action();
        return a;
    }

    public static async Task<A> Effect<A>(this A a, Func<A, Task> action)
    {
        await action(a);
        return a;
    }

    public static async Task<A> Effect<A>(this Task<A> a, Action action)
    {
        var result = await a;
        action();
        return result;
    }

    public static async Task<A> Effect<A>(this Task<A> a, Action<A> action)
    {
        var result = await a;
        action(result);
        return result;
    }

    public static async Task<A> Effect<A>(this Task<A> a, Func<Task> action)
    {
        var result = await a;
        await action();
        return result;
    }

    public static async Task<A> Effect<A>(this Task<A> a, Func<A, Task> action)
    {
        var result = await a;
        await action(result);
        return result;
    }

    public static async Task<(A, B)> Effect<A, B>(this Task<(A, B)> ab, Func<A, B, Task> action)
    {
        var result = await ab;
        await action(result.Item1, result.Item2);
        return result;
    }
}