using System;
using System.Threading.Tasks;

public static class Operators
{
    public static B Pipe<A, B>(this A a, Func<A, B> func) => func(a);
    public static async Task<B> Pipe<A, B>(this Task<A> a, Func<A, B> func) => func(await a);
    public static async Task<B> Pipe<A, B>(this Task<A> a, Func<A, Task<B>> func) => await func(await a);
}