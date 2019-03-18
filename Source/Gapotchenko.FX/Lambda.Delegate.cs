using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    partial class Lambda
    {
        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action Delegate(Action action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T> Delegate<T>(Action<T> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2> Delegate<T1, T2>(Action<T1, T2> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3> Delegate<T1, T2, T3>(Action<T1, T2, T3> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4> Delegate<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5> Delegate<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6> Delegate<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7> Delegate<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Delegate<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of a lambda function.</typeparam>
        /// <param name="action">The lambda function.</param>
        /// <returns>The lambda function specified by an <paramref name="action"/> parameter.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action) => action;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<TResult> Delegate<TResult>(Func<TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T, TResult> Delegate<T, TResult>(Func<T, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, TResult> Delegate<T1, T2, TResult>(Func<T1, T2, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, TResult> Delegate<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, TResult> Delegate<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, TResult> Delegate<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, TResult> Delegate<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func) => func;

        /// <summary>
        /// Infers the delegate type of a specified lambda function.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of a lambda function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of a lambda function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of a lambda function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of a lambda function.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of a lambda function.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of a lambda function.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of a lambda function.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of a lambda function.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of a lambda function.</typeparam>
        /// <typeparam name="T12">The type of the twelveth parameter of a lambda function.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of a lambda function.</typeparam>
        /// <typeparam name="TResult">The type of the result of a lambda function.</typeparam>
        /// <param name="func">The lambda function.</param>
        /// <returns>The lambda function specified by a <paramref name="func"/> parameter.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Delegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func) => func;
    }
}
