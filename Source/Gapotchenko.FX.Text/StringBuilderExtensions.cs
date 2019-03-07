using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Text
{
    /// <summary>
    /// <see cref="StringBuilder"/> extensions.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the elements of an object array, using the specified separator between each element.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="separator">The string to use as a separator.</param>
        /// <param name="values">An array that contains the elements to append.</param>
        /// <returns>A reference to string builder after the append operation has completed.</returns>
        public static StringBuilder AppendJoin(this StringBuilder sb, string separator, params object[] values)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length == 0)
                return sb;

            // Append the first value.
            sb.Append(values[0]);

            // Append remaining values.
            for (int i = 1; i < values.Length; ++i)
            {
                sb.Append(separator);
                sb.Append(values[i]);
            }

            return sb;
        }

        /// <summary>
        /// Appends the members of a collection, using the specified separator between each element.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="separator">The string to use as a separator.</param>
        /// <param name="values">A collection that contains the objects to append.</param>
        /// <returns>A reference to string builder after the append operation has completed.</returns>
        public static StringBuilder AppendJoin<T>(this StringBuilder sb, string separator, IEnumerable<T> values)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return sb;

                // Append the first value.
                sb.Append(enumerator.Current);

                // Append remaining values.
                while (enumerator.MoveNext())
                {
                    sb.Append(separator);
                    sb.Append(enumerator.Current);
                }
            }

            return sb;
        }

        /// <summary>
        /// Appends all the elements of a string array, using the specified separator between each element.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="separator">The string to use as a separator.</param>
        /// <param name="values">An array that contains the elements to append.</param>
        /// <returns>A reference to string builder after the append operation has completed.</returns>
        public static StringBuilder AppendJoin(this StringBuilder sb, string separator, params string[] values)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length == 0)
                return sb;

            // Append the first value.
            sb.Append(values[0]);

            // Append remaining values.
            for (int i = 1; i < values.Length; ++i)
            {
                sb.Append(separator);
                sb.Append(values[i]);
            }

            return sb;
        }

        /// <summary>
        /// Appends the elements of an object array, using the specified separator between each element.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="separator">The character to use as a separator.</param>
        /// <param name="values">An array that contains the elements to append.</param>
        /// <returns>A reference to string builder after the append operation has completed.</returns>
        public static StringBuilder AppendJoin(this StringBuilder sb, char separator, params object[] values)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length == 0)
                return sb;

            // Append the first value.
            sb.Append(values[0]);

            // Append remaining values.
            for (int i = 1; i < values.Length; ++i)
            {
                sb.Append(separator);
                sb.Append(values[i]);
            }

            return sb;
        }

        /// <summary>
        /// Appends the members of a collection, using the specified separator between each element.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="separator">The character to use as a separator.</param>
        /// <param name="values">A collection that contains the objects to append.</param>
        /// <returns>A reference to string builder after the append operation has completed.</returns>
        public static StringBuilder AppendJoin<T>(this StringBuilder sb, char separator, IEnumerable<T> values)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return sb;

                // Append the first value.
                sb.Append(enumerator.Current);

                // Append remaining values.
                while (enumerator.MoveNext())
                {
                    sb.Append(separator);
                    sb.Append(enumerator.Current);
                }
            }

            return sb;
        }

        /// <summary>
        /// Appends all the elements of a string array, using the specified separator between each element.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="separator">The character to use as a separator.</param>
        /// <param name="values">An array that contains the elements to append.</param>
        /// <returns>A reference to string builder after the append operation has completed.</returns>
        public static StringBuilder AppendJoin(this StringBuilder sb, char separator, params string[] values)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length == 0)
                return sb;

            // Append the first value.
            sb.Append(values[0]);

            // Append remaining values.
            for (int i = 1; i < values.Length; ++i)
            {
                sb.Append(separator);
                sb.Append(values[i]);
            }

            return sb;
        }
    }
}
