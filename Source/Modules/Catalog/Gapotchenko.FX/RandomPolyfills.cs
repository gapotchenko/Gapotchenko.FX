// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NET6_0_OR_GREATER
#define TFF_RANDOM_SHARED
#endif

#if NET8_0_OR_GREATER
#define TFF_RANDOM_SHUFFLE
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill extension members for <see cref="Random"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RandomPolyfills
{
    /// <inheritdoc cref="RandomPolyfills"/>
    extension(Random)
    {
        /// <summary>
        /// Provides a thread-safe <see cref="Random"/> instance that may be used concurrently from any thread.
        /// </summary>
#if TFF_RANDOM_SHARED
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static Random Shared
        {
#if TFF_RANDOM_SHARED
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get =>
#if TFF_RANDOM_SHARED
              Random.Shared;
#else
              ThreadSafeRandom.Instance;
#endif
        }
    }

#if !TFF_RANDOM_SHARED
    sealed class ThreadSafeRandom : Random
    {
        public static ThreadSafeRandom Instance { get; } = new();

        public override int Next() => Impl.Next();

        public override int Next(int maxValue) => Impl.Next(maxValue);

        public override int Next(int minValue, int maxValue) => Impl.Next(minValue, maxValue);

        public override void NextBytes(byte[] buffer) => Impl.NextBytes(buffer);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public override void NextBytes(Span<byte> buffer) => Impl.NextBytes(buffer);
#endif

        protected override double Sample() => Impl.NextDouble();

        [field: ThreadStatic]
        static Random Impl => field ??= new();
    }
#endif

    /// <summary>
    /// Performs an in-place shuffle of a span.
    /// </summary>
    /// <typeparam name="T">The type of span.</typeparam>
    /// <param name="random">The <see cref="Random"/> instance.</param>
    /// <param name="values">The span to shuffle.</param>
    /// <remarks>
    /// This method uses <see cref="Random.Next(int, int)"/> to choose values for shuffling.
    /// This method is an O(n) operation.
    /// </remarks>
#if TFF_RANDOM_SHUFFLE
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static void Shuffle<T>(
#if !TFF_RANDOM_SHUFFLE
        this
#endif
        Random random, Span<T> values)
    {
#if TFF_RANDOM_SHUFFLE
        random.Shuffle(values);
#else
        ArgumentNullException.ThrowIfNull(random);

        for (int i = 0, n = values.Length; i < n - 1; ++i)
        {
            int j = random.Next(i, n);

            if (j != i)
            {
                var temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
        }
#endif
    }
}
