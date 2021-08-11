using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class TaskUtils
{
    public static Task<T[]> ToTask<T>(this IEnumerable<Task<T>> tasks) => Task.WhenAll(tasks);
    public static async Task<B> Map<A, B>(this Task<A> task, Func<A, B> mapper) => mapper(await task);
    public static async Task<B> FlatMap<A, B>(this Task<A> task, Func<A, Task<B>> mapper) => await mapper(await task);
    public static async Task FlatMap<A>(this Task<A> task, Func<A, Task> mapper) => await mapper(await task);
}