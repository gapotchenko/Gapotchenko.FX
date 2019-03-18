using System;
using System.Linq.Expressions;

namespace Gapotchenko.FX
{
    partial class Fn
    {
        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action> Expression(Expression<Action> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T>> Expression<T>(Expression<Action<T>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2>> Expression<T1, T2>(Expression<Action<T1, T2>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2, T3>> Expression<T1, T2, T3>(Expression<Action<T1, T2, T3>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2, T3, T4>> Expression<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2, T3, T4, T5>> Expression<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2, T3, T4, T5, T6>> Expression<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>> Expression<T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda expression.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> Expression<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda expression.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression) => expression;

        /// <summary>
        /// Infers the type of a specified lambda expression.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda expression.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda expression.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda expression.</typeparam>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> expression) => expression;
    }
}
