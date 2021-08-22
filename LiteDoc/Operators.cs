using System;
using System.Threading.Tasks;

public static class Pipes
{
    public static B Pipe<A, B>(this A a, Func<A, B> func) => func(a);
    public static async Task<B> Pipe<A, B>(this Task<A> a, Func<A, B> func) => func(await a);
    public static async Task<B> Pipe<A, B>(this Task<A> a, Func<A, Task<B>> func) => await func(await a);
}

public static class Tuples
{
    public static (A, B) With<A, B>(this A a, B b) => (a, b);
    public static async Task<(A, B)> With<A, B>(this Task<A> a, B b) => (await a, b);
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
}