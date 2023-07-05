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
    protected const int TestData_SampleSize = 3;

    public static IEnumerable<object[]> TestData_Capacity_ValidValues()
    {
        yield return new object[] { 0 };
        yield return new object[] { 1 };
        yield return new object[] { 17 };
    }

    public static IEnumerable<object[]> TestData_Capacity_InvalidValues()
    {
        yield return new object[] { -1 };
        yield return new object[] { -2 };
        yield return new object[] { int.MinValue };
    }

    /// <summary>
    /// Adds the specified data to <see cref="Deque{T}"/> while preserving the layout of its internal storage.
    /// </summary>
    protected static void TestData_FillDeque(Deque<T> deque, IEnumerable<T> collection)
    {
        foreach (var i in collection)
            deque.PushBack(i);
    }

    /// <summary>
    /// Enumerates all possible combinations of
    /// internal storage layouts of a <see cref="Deque{T}"/>
    /// for the specified collection size interval.
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
    /// for the specified collection size interval.
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
    /// and indices
    /// for the specified collection size interval.
    /// </summary>
    public static IEnumerable<object[]> TestData_SizeAndDequeLayoutCombinationsWithIndex(int minSize, int maxSize)
    {
        var sampleQueue = new Deque<T>(); // used for capacity calculation only
        for (int size = minSize; size <= maxSize; ++size)
        {
            var capacity = sampleQueue.EnsureCapacity(size);
            foreach (int layoutOffset in TestData_EnumerateDequeLayoutOffsets(capacity))
                for (var index = 0; index < size; ++index)
                    yield return new object[] { size, TestData_MakeDequeLayout(size, layoutOffset), index };
        }
    }

    /// <summary>
    /// Enumerates all possible combinations of collection sizes,
    /// internal storage layouts of a <see cref="Deque{T}"/>,
    /// indices,
    /// and counts
    /// for the specified collection size interval.
    /// </summary>
    public static IEnumerable<object[]> TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount(int minSize, int maxSize, int minCount)
    {
        var sampleQueue = new Deque<T>(); // used for capacity calculation only
        for (int size = minSize; size <= maxSize; ++size)
        {
            var capacity = sampleQueue.EnsureCapacity(size);
            foreach (int layoutOffset in TestData_EnumerateDequeLayoutOffsets(capacity))
                for (var index = 0; index < size; ++index)
                    for (var count = minCount; index + count <= size; ++count)
                        yield return new object[] { size, TestData_MakeDequeLayout(size, layoutOffset), index, count };
        }
    }

    /// <summary>
    /// Enumerates all possible combinations of collection sizes,
    /// internal storage layouts of a <see cref="Deque{T}"/>,
    /// and insertion indices
    /// for the specified collection size.
    /// </summary>
    public static IEnumerable<object[]> TestData_SizeAndDequeLayoutCombinationsWithInsertionIndex(int minSize, int maxSize)
    {
        var sampleQueue = new Deque<T>(); // used for capacity calculation only
        for (int size = minSize; size <= maxSize; ++size)
        {
            var capacity = sampleQueue.EnsureCapacity(size);
            foreach (int layoutOffset in TestData_EnumerateDequeLayoutOffsets(capacity))
                for (var index = 0; index <= size; ++index)
                    yield return new object[] { size, TestData_MakeDequeLayout(size, layoutOffset), index };
        }
    }

    /// <summary>
    /// Enumerates all possible combinations of collection sizes,
    /// internal storage layouts of a <see cref="Deque{T}"/>,
    /// and insertion ranges
    /// for the specified collection size and insertion range length intervals.
    /// </summary>
    public static IEnumerable<object[]> TestData_SizeAndDequeLayoutCombinationsWithInsertionIndexAndCount(
        int minSize, int maxSize,
        int minCount, int maxCount)
    {
        var sampleQueue = new Deque<T>(); // used for capacity calculation only
        for (int size = minSize; size <= maxSize; ++size)
        {
            var capacity = sampleQueue.EnsureCapacity(size);
            foreach (int layoutOffset in TestData_EnumerateDequeLayoutOffsets(capacity))
                for (var index = 0; index <= size; ++index)
                    for (int count = minCount; count <= maxCount; ++count)
                        yield return new object[] { size, TestData_MakeDequeLayout(size, layoutOffset), index, count };
        }
    }

    /// <summary>
    /// Enumerates all possible combinations of collection sizes,
    /// internal storage layouts of a <see cref="Deque{T}"/>,
    /// and range lengths
    /// for the specified collection size and range length intervals.
    /// </summary>
    public static IEnumerable<object[]> TestData_SizeAndDequeLayoutCombinationsWithInsertionCount(
        int minSize, int maxSize,
        int minCount, int maxCount)
    {
        var sampleQueue = new Deque<T>(); // used for capacity calculation only
        for (int size = minSize; size <= maxSize; ++size)
        {
            var capacity = sampleQueue.EnsureCapacity(size);
            foreach (int layoutOffset in TestData_EnumerateDequeLayoutOffsets(capacity))
                for (int count = minCount; count <= maxCount; ++count)
                    yield return new object[] { size, TestData_MakeDequeLayout(size, layoutOffset), count };
        }
    }

    //-----------------------------------------------------------------------

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
