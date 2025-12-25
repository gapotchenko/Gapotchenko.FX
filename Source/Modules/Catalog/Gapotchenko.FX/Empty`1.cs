#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Gapotchenko.FX;

/// <summary>
/// Provides typed constructions related to a functional notion of emptiness.
/// </summary>
/// <typeparam name="T">The type.</typeparam>
public static class Empty<T>
{
    /// <summary>
    /// Returns an empty array.
    /// </summary>
    [Obsolete("Use System.Array.Empty<T>() instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T[] Array =>
#pragma warning disable IDE0301 // Simplify collection initialization
        System.Array.Empty<T>();
#pragma warning restore IDE0301 // Simplify collection initialization

    /// <summary>
    /// Returns an empty <see cref="Task{TResult}"/> that has already completed successfully with the default result.
    /// </summary>
#if TFF_TASK_FROMRESULT
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Task<T> Task => TaskFactory.Task;

    static class TaskFactory
    {
        public static readonly Task<T> Task = System.Threading.Tasks.Task.FromResult(default(T)!);
    }
}
