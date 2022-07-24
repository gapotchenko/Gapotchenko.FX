using System.ComponentModel;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// The base class for <see cref="IDataEncoding"/> implementations.
/// </summary>
public abstract class DataEncoding : IDataEncoding
{
    /// <inheritdoc/>
    public float Efficiency => EfficiencyCore;

    /// <summary>
    /// Gets the average encoding efficiency.
    /// </summary>
    protected abstract float EfficiencyCore { get; }

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public float MaxEfficiency => MaxEfficiencyCore;

    /// <summary>
    /// Gets the maximum encoding efficiency.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual float MaxEfficiencyCore => EfficiencyCore;

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public float MinEfficiency => MinEfficiencyCore;

    /// <summary>
    /// Gets the minimum encoding efficiency.                   
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual float MinEfficiencyCore => EfficiencyCore;

    /// <summary>
    /// Validates and returns effective options to use.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The effective encoding options to use.</returns>
    protected virtual DataEncodingOptions GetEffectiveOptions(DataEncodingOptions options)
    {
        const DataEncodingOptions PaddingConflictMask = DataEncodingOptions.Padding | DataEncodingOptions.NoPadding;
        if ((options & PaddingConflictMask) == PaddingConflictMask)
        {
            throw new ArgumentException(
                string.Format(
                    "'{0}' and '{1}' options cannot be used simultaneously.",
                    nameof(DataEncodingOptions.Padding),
                    nameof(DataEncodingOptions.NoPadding)),
                nameof(options));
        }

        return options;
    }

    /// <inheritdoc/>
    public byte[] EncodeData(ReadOnlySpan<byte> data) => EncodeData(data, DataEncodingOptions.None);

    /// <inheritdoc/>
    public byte[] EncodeData(ReadOnlySpan<byte> data, DataEncodingOptions options) => EncodeDataCore(data, GetEffectiveOptions(options));

    /// <summary>
    /// Provides the core implementation of data encoding.
    /// </summary>
    /// <param name="data">The input data.</param>
    /// <param name="options">The options.</param>
    /// <returns>The encoded output data.</returns>
    protected abstract byte[] EncodeDataCore(ReadOnlySpan<byte> data, DataEncodingOptions options);

    /// <inheritdoc/>
    public byte[] DecodeData(ReadOnlySpan<byte> data) => DecodeData(data, DataEncodingOptions.None);

    /// <inheritdoc/>
    public byte[] DecodeData(ReadOnlySpan<byte> data, DataEncodingOptions options) => DecodeDataCore(data, GetEffectiveOptions(options));

    /// <summary>
    /// Provides the core implementation of data decoding.
    /// </summary>
    /// <param name="data">The encoded input data.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded output data.</returns>
    protected abstract byte[] DecodeDataCore(ReadOnlySpan<byte> data, DataEncodingOptions options);

    /// <inheritdoc/>
    public virtual bool CanStream => true;

    /// <summary>
    /// Validates and returns effective streaming options to use.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The effective encoding options to use.</returns>
    protected virtual DataEncodingOptions GetEffectiveStreamingOptions(DataEncodingOptions options)
    {
        if (!CanStream)
        {
            throw new NotSupportedException(
                string.Format(
                    "{0} encoding does not support streaming operations.",
                    this));
        }

        return GetEffectiveOptions(options);
    }

    /// <inheritdoc/>
    public abstract Stream CreateEncoder(Stream outputStream, DataEncodingOptions options = DataEncodingOptions.None);

    /// <inheritdoc/>
    public abstract Stream CreateDecoder(Stream inputStream, DataEncodingOptions options = DataEncodingOptions.None);

    /// <inheritdoc/>
    public int Padding => PaddingCore;

    /// <summary>
    /// Gets the number of symbols used for padding of an encoded data representation.
    /// </summary>
    protected virtual int PaddingCore => 1;

    /// <inheritdoc/>
    public bool CanPad => Padding > 1;

    /// <inheritdoc/>
    public virtual bool PrefersPadding => CanPad;

    /// <summary>
    /// Gets count of leading zeros.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The count of leading zeros in <paramref name="data"/>.</returns>
    protected static int GetLeadingZeroCount(ReadOnlySpan<byte> data)
    {
        int count = 0;
        foreach (var b in data)
        {
            if (b != 0)
                break;
            checked { ++count; }
        }
        return count;
    }
}
