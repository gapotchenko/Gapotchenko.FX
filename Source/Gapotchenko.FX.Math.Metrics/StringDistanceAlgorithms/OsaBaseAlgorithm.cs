using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.StringDistanceAlgorithms;

using Math = System.Math;

abstract class OsaBaseAlgorithm : StringDistanceAlgorithm
{
    protected static int MeasureCore<T>(
        IEnumerable<T> a,
        IEnumerable<T> b,
        in ValueInterval<int> range,
        bool allowReplacements,
        bool allowTranspositions,
        IEqualityComparer<T>? equalityComparer,
        CancellationToken cancellationToken)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));
        if (b == null)
            throw new ArgumentNullException(nameof(b));
        ValidateRange(range);

        var workingRange = range.Intersect(ValueInterval.Inclusive<int>(0, null));
        if (workingRange.IsEmpty)
            return range.Clamp(0).Value;

        if (ReferenceEquals(a, b))
            return 0;

        var sRow = b.ReifyList();
        var sCol = a.ReifyList();

        int rowLen = sRow.Count;
        int colLen = sCol.Count;

        if (rowLen == 0)
            return colLen;
        if (colLen == 0)
            return rowLen;

        equalityComparer ??= EqualityComparer<T>.Default;

        // Create and initialize the row vector.
        var row = new int[rowLen + 1];
        var preRow = new int[rowLen + 1];
        var prePreRow = allowTranspositions ? new int[rowLen + 1] : null;

        for (int rowIdx = 1; rowIdx <= rowLen; rowIdx++)
            preRow[rowIdx] = rowIdx;

        // For each column.
        for (int colIdx = 1; colIdx <= colLen; colIdx++)
        {
            // Set the first element to the column number.
            row[0] = colIdx;

            var col_j = sCol[colIdx - 1];
            int bestAtRow = colIdx;

            // For each row.
            for (int rowIdx = 1; rowIdx <= rowLen; rowIdx++)
            {
                var row_i = sRow[rowIdx - 1];

                int currentDistance;
                if (equalityComparer.Equals(row_i, col_j))
                {
                    currentDistance = preRow[rowIdx - 1];
                }
                else
                {
                    // Find minimum.
                    currentDistance = Math.Min(preRow[rowIdx] + 1, row[rowIdx - 1] + 1);

                    if (allowReplacements)
                    {
                        currentDistance = Math.Min(currentDistance, preRow[rowIdx - 1] + 1);
                    }

                    if (prePreRow != null &&
                        rowIdx > 1 && colIdx > 1 &&
                        equalityComparer.Equals(row_i, sCol[colIdx - 2]) &&
                        equalityComparer.Equals(sRow[rowIdx - 2], col_j))
                    {
                        currentDistance = Math.Min(currentDistance, prePreRow[rowIdx - 2] + 1);
                    }
                }

                row[rowIdx] = currentDistance;

                bestAtRow = Math.Min(bestAtRow, currentDistance);
            }

            // TODO
            //if (bestAtRow >= maxDistance)
            //    return maxDistance.Value;

            cancellationToken.ThrowIfCancellationRequested();

            // Swap the vectors.
            (preRow, row) = (row, preRow);

            if (prePreRow != null)
                (prePreRow, row) = (row, prePreRow);
        }

        var distance = preRow[rowLen];
        return range.Clamp(distance).Value;
    }
}
