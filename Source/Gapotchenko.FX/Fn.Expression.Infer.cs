using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Gapotchenko.FX;

partial class Fn
{
    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action>? Expression(Expression<Action>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T>>? Expression<T>(Expression<Action<T>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2>>? Expression<T1, T2>(Expression<Action<T1, T2>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3>>? Expression<T1, T2, T3>(Expression<Action<T1, T2, T3>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4>>? Expression<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>>? expression) => expression;

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
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5>>? Expression<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>>? expression) => expression;

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
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6>>? Expression<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>>? expression) => expression;

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
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>>? Expression<T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T1, T2, T3, T4, T5, T6, T7>>? expression) => expression;

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
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>>? expression) => expression;

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
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>? expression) => expression;

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
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T16">The type of the sixteenth parameter of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<TResult>>? Expression<TResult>(Expression<Func<TResult>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T, TResult>>? Expression<T, TResult>(Expression<Func<T, TResult>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, TResult>>? Expression<T1, T2, TResult>(Expression<Func<T1, T2, TResult>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, TResult>>? Expression<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, TResult>>? Expression<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, TResult>>? Expression<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>>? expression) => expression;

    /// <summary>
    /// Infers the type of a specified lambda expression.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of a lambda expression.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of a lambda expression.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of a lambda expression.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>>? Expression<T1, T2, T3, T4, T5, T6, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, TResult>>? expression) => expression;

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
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")] 
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>? expression) => expression;

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
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>? expression) => expression;

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
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>? expression) => expression;

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
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>? expression) => expression;

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
    /// <typeparam name="T11">The type of the eleventh parameter of a lambda expression.</typeparam>
    /// <typeparam name="T12">The type of the twelfth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T13">The type of the thirteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T14">The type of the fourteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T15">The type of the fifteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="T16">The type of the sixteenth parameter of a lambda expression.</typeparam>
    /// <typeparam name="TResult">The type of the result of a lambda expression.</typeparam>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The lambda expression specified by an <paramref name="expression"/> parameter.</returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>? Expression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>? expression) => expression;
}
