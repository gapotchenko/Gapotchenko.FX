// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    /// <summary>
    /// Adds the specified data to <see cref="Deque{T}"/> while preserving the layout of its internal storage.
    /// </summary>
    protected static void TestData_FillDeque(Deque<T> deque, IEnumerable<T> collection)
    {
        foreach (var i in collection)
            deque.PushBack(i);
    }

    /// <summary>
    /// Enumerates all possible combinations of collection sizes
    /// and internal storage layouts of a <see cref="Deque{T}"/>
    /// for the specified max collection size.
    /// </summary>
    public static IEnumerable<object[]> TestData_DequeLayoutCombinations(int minSize, int maxSize)
    {
        var sampleQueue = new Deque<T>(); // used for capacity calculation only
        var handledCapacities = new HashSet<int>();

        for (int size = minSize; size <= maxSize; ++size)
        {
            var capacity = sampleQueue.EnsureCapacity(size);
            if (!handledCapacities.Add(capacity))
                continue;

            foreach (int layoutOffset in TestData_EnumerateDequeLayoutOffsets(capacity))
                yield return new object[] { TestData_MakeDequeLayout(capacity, layoutOffset) };
        }
    }

    /// <summary>
    /// Enumerates all possible combinations of collection sizes
    /// and internal storage layouts of a <see cref="Deque{T}"/>
    /// for the specified max collection size.
    /// </summary>
    public static IEnumerable<object[]> TestData_SizeAndDequeLayoutCombinations(int minSize, int maxSize)
    {
        var sampleQueue = new Deque<T>(); // used for capacity calculation only
        for (int size = minSize; size <= maxSize; ++size)
        {
            var capacity = sampleQueue.EnsureCapacity(size);
            foreach (int layoutOffset in TestData_EnumerateDequeLayoutOffsets(capacity))
                yield return new object[] { size, TestData_MakeDequeLayout(size, layoutOffset) };
        }
    }

    /// <summary>
    /// Enumerates all possible combinations of collection sizes,
    /// internal storage layouts of a <see cref="Deque{T}"/>,
    /// and range lengths
    /// for the specified max collection size and the max range length.
    /// </summary>
    public static IEnumerable<object[]> TestData_SizeAndDequeLayoutCombinationsWithRangeLength(
        int minSize, int maxSize,
        int minLength, int maxLength)
    {
        var sampleQueue = new Deque<T>(); // used for capacity calculation only
        for (int size = minSize; size <= maxSize; ++size)
        {
            var capacity = sampleQueue.EnsureCapacity(size);
            foreach (int layoutOffset in TestData_EnumerateDequeLayoutOffsets(capacity))
                for (int length = minLength; length <= maxLength; ++length)
                    yield return new object[] { size, TestData_MakeDequeLayout(size, layoutOffset), length };
        }
    }

    /// <summary>
    /// Enumerates layout offsets that target specific cells of the deque's internal storage array
    /// to be able to produce all combinations of all possible data layouts in the array.
    /// </summary>
    static IEnumerable<int> TestData_EnumerateDequeLayoutOffsets(int capacity)
    {
        if (capacity <= 1)
        {
            yield return 0;
        }
        else
        {
            int half = capacity / 2;
            for (var offset = -half; offset < half; ++offset)
                yield return offset;
        }
    }

    static Deque<T> TestData_MakeDequeLayout(int size, int layoutOffset)
    {
        var deque = new Deque<T>();
        deque.EnsureCapacity(size);

        // The layout offset targets specific cells at the deque's internal storage array.
        if (layoutOffset >= 0)
        {
            // Move deque's pointers forward, one by one cell at a time.
            for (var i = 0; i < layoutOffset; i++)
            {
                deque.PushBack(default!);
                deque.PopFront();
            }
        }
        else
        {
            // Move deque's pointers backward, one by one cell at a time.
            for (var i = 0; i < -layoutOffset; i++)
            {
                deque.PushFront(default!);
                deque.PopBack();
            }
        }

        return deque;
    }
}
