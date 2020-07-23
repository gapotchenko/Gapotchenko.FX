using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#nullable enable
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Gapotchenko.FX
{
    partial class Fn
    {
        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action>? Expression(Action? action) =>
            action == null ?
                (Expression<Action>?)null :
                () => action();

        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <typeparam name="T">The type of the parameter of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action<T>>? Expression<T>(Action<T>? action) =>
            action == null ?
                (Expression<Action<T>>?)null :
                (val) => action(val);

        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action<T1, T2>>? Expression<T1, T2>(Action<T1, T2>? action) =>
            action == null ?
                (Expression<Action<T1, T2>>?)null :
                (val1, val2) => action(val1, val2);

        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action<T1, T2, T3>>? Expression<T1, T2, T3>(Action<T1, T2, T3>? action) =>
            action == null ?
                (Expression<Action<T1, T2, T3>>?)null :
                (val1, val2, val3) => action(val1, val2, val3);

        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action<T1, T2, T3, T4>>? Expression<T1, T2, T3, T4>(Action<T1, T2, T3, T4>? action) =>
            action == null ?
                (Expression<Action<T1, T2, T3, T4>>?)null :
                (val1, val2, val3, val4) => action(val1, val2, val3, val4);

        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action<T1, T2, T3, T4, T5>>? Expression<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5>? action) =>
            action == null ?
                (Expression<Action<T1, T2, T3, T4, T5>>?)null :
                (val1, val2, val3, val4, val5) => action(val1, val2, val3, val4, val5);

        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action<T1, T2, T3, T4, T5, T6>>? Expression<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6>? action) =>
            action == null ?
                (Expression<Action<T1, T2, T3, T4, T5, T6>>?)null :
                (val1, val2, val3, val4, val5, val6) => action(val1, val2, val3, val4, val5, val6);

        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>>? Expression<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7>? action) =>
            action == null ?
                (Expression<Action<T1, T2, T3, T4, T5, T6, T7>>?)null :
                (val1, val2, val3, val4, val5, val6, val7) => action(val1, val2, val3, val4, val5, val6, val7);

        /// <summary>
        /// Creates lambda expression for a specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda expression.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="action"/>.</returns>
        [return: NotNullIfNotNull("action")]
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8>? action) =>
            action == null ?
                (Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>>?)null :
                (val1, val2, val3, val4, val5, val6, val7, val8) => action(val1, val2, val3, val4, val5, val6, val7, val8);

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<TResult>>? Expression<TResult>(Func<TResult>? func) =>
            func == null ?
                (Expression<Func<TResult>>?)null :
                () => func();

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="T">The type of the parameter of a lambda expression.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<T, TResult>>? Expression<T, TResult>(Func<T, TResult>? func) =>
            func == null ?
                (Expression<Func<T, TResult>>?)null :
                (val) => func(val);

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<T1, T2, TResult>>? Expression<T1, T2, TResult>(Func<T1, T2, TResult>? func) =>
            func == null ?
                (Expression<Func<T1, T2, TResult>>?)null :
                (val1, val2) => func(val1, val2);

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<T1, T2, T3, TResult>>? Expression<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult>? func) =>
            func == null ?
                (Expression<Func<T1, T2, T3, TResult>>?)null :
                (val1, val2, val3) => func(val1, val2, val3);

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<T1, T2, T3, T4, TResult>>? Expression<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult>? func) =>
            func == null ?
                (Expression<Func<T1, T2, T3, T4, TResult>>?)null :
                (val1, val2, val3, val4) => func(val1, val2, val3, val4);

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>>? Expression<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult>? func) =>
            func == null ?
                (Expression<Func<T1, T2, T3, T4, T5, TResult>>?)null :
                (val1, val2, val3, val4, val5) => func(val1, val2, val3, val4, val5);

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>>? Expression<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult>? func) =>
            func == null ?
                (Expression<Func<T1, T2, T3, T4, T5, T6, TResult>>?)null :
                (val1, val2, val3, val4, val5, val6) => func(val1, val2, val3, val4, val5, val6);

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda expression.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult>? func) =>
            func == null ?
                (Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>?)null :
                (val1, val2, val3, val4, val5, val6, val7) => func(val1, val2, val3, val4, val5, val6, val7);

        /// <summary>
        /// Creates lambda expression for a specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda expression.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda expression.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
        /// <returns>A lambda expression for the specified <paramref name="func">function</paramref>.</returns>
        [return: NotNullIfNotNull("func")]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>? func) =>
            func == null ?
                (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>?)null :
                (val1, val2, val3, val4, val5, val6, val7, val8) => func(val1, val2, val3, val4, val5, val6, val7, val8);
    }
}
