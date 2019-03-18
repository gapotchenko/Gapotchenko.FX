using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides operations related to a functional notion of emptiness.
    /// </summary>
    public static class Empty
    {
        /// <summary>
        /// Nullifies an empty collection.
        /// </summary>
        /// <typeparam name="TSource">The type of the input collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>The source or a null collection if the source is empty or null.</returns>
        public static TSource Nullify<TSource>(TSource source) where TSource : class, ICollection
        {
            if (source == null || source.Count == 0)
                return null;
            else
                return source;
        }

        /// <summary>
        /// Nullifies an empty sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>The source or a null sequence if the source is empty or null.</returns>
        public static IEnumerable<TSource> Nullify<TSource>(IEnumerable<TSource> source)
        {
            if (source == null || !source.Any())
                return null;
            else
                return source;
        }

        /// <summary>
        /// Nullifies an empty array.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input array.</typeparam>
        /// <param name="source">The source array.</param>
        /// <returns>The source or a null array if the source is empty or null.</returns>
        public static TSource[] Nullify<TSource>(TSource[] source)
        {
            if (source == null || source.Length == 0)
                return null;
            else
                return source;
        }

        /// <summary>
        /// Nullifies an empty string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>The source or a null string if the source is empty or null.</returns>
        public static string Nullify(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            else
                return source;
        }

        /// <summary>
        /// Nullifies a zero <see cref="SByte"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value or a <c>null</c> if the value is zero.</returns>
        [CLSCompliant(false)]
        public static sbyte? Nullify(sbyte value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Nullifies a zero <see cref="Byte"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value or a <c>null</c> if the value is zero.</returns>
        public static byte? Nullify(byte value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Nullifies a zero <see cref="Int16"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value or a <c>null</c> if the value is zero.</returns>
        public static short? Nullify(short value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Nullifies a zero <see cref="UInt16"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value or a <c>null</c> if the value is zero.</returns>
        [CLSCompliant(false)]
        public static ushort? Nullify(ushort value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Nullifies a zero <see cref="Int32"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value or a <c>null</c> if the value is zero.</returns>
        public static int? Nullify(int value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Nullifies a zero <see cref="UInt32"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value or a <c>null</c> if the value is zero.</returns>
        [CLSCompliant(false)]
        public static uint? Nullify(uint value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Nullifies a zero <see cref="Int64"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value or a <c>null</c> if the value is zero.</returns>
        public static long? Nullify(long value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Nullifies a zero <see cref="UInt64"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value or a <c>null</c> if the value is zero.</returns>
        [CLSCompliant(false)]
        public static ulong? Nullify(ulong value)
        {
            if (value == 0)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Nullifies a string when it is empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>The source or a null string if the source is empty, or consists only of white-space characters.</returns>
        public static string NullifyWhiteSpace(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            else
                return source;
        }

        /// <summary>
        /// Nullifies an empty function in terms of lambda calculus provided by <see cref="Fn"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The value of <paramref name="action"/> or <c>null</c> if it represents an empty function in terms of lambda calculus.</returns>
        public static Action Nullify(Action action)
        {
            if (action == Fn.Empty)
                return null;
            else
                return action;
        }

        /// <summary>
        /// Nullifies a default function in terms of lambda calculus provided by <see cref="Fn"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>The value of <paramref name="func"/> or <c>null</c> if it represents a default function in terms of lambda calculus.</returns>
        public static Func<T> Nullify<T>(Func<T> func)
        {
            if (func == Fn<T>.Default)
                return null;
            else
                return func;
        }

        /// <summary>
        /// Nullifies an identity function in terms of lambda calculus provided by <see cref="Fn"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>The value of <paramref name="func"/> or <c>null</c> if it represents an identity function in terms of lambda calculus.</returns>
        public static Func<T, T> Nullify<T>(Func<T, T> func)
        {
            if (func == Fn<T>.Identity)
                return null;
            else
                return func;
        }
    }
}
