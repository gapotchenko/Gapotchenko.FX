namespace Gapotchenko.FX.Data.Integrity.Checksum
{
    /// <summary>
    /// Represents a generic iterator for checksum computation.
    /// </summary>
    /// <typeparam name="T">The type of the checksum value.</typeparam>
    public interface IChecksumIterator<T> : IChecksumIterator
        where T : struct
    {
        /// <summary>
        /// Finalizes the checksum computation after the last data is processed.
        /// </summary>
        /// <returns>The computed checksum.</returns>
        new T ComputeFinal();
    }
}
