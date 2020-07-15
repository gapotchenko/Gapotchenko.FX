namespace System
{
    /// <summary>
    /// <para>
    /// Supports cloning, which creates a new instance of class <typeparamref name="T"/> with the same value as an existing instance.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The class of a cloneable object.</typeparam>
    public interface ICloneable<out T> : ICloneable where T : class
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        new T Clone();
    }
}
