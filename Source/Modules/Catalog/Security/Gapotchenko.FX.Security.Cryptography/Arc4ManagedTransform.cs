using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography;

sealed class Arc4ManagedTransform(byte[] key) : ICryptoTransform
{
    byte[]? m_State = CreateState(key);
    byte m_X, m_Y;

    public void Dispose()
    {
        if (m_State != null)
        {
            m_X = 0;
            m_Y = 0;
            Array.Clear(m_State, 0, m_State.Length);
            m_State = null;
        }
    }

    public bool CanReuseTransform => false;

    public bool CanTransformMultipleBlocks => true;

    public int InputBlockSize => 1;

    public int OutputBlockSize => 1;

    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
    {
        CheckInputParameters(inputBuffer, inputOffset, inputCount);

        ArgumentNullException.ThrowIfNull(outputBuffer);
        ArgumentOutOfRangeException.ThrowIfNegative(outputOffset);
        if (outputOffset > outputBuffer.Length - inputCount)
        {
            throw new ArgumentException(
                "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.",
                nameof(outputBuffer));
        }

        return TransformBlockCore(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
    }

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        CheckInputParameters(inputBuffer, inputOffset, inputCount);

        byte[] output = new byte[inputCount];
        TransformBlockCore(inputBuffer, inputOffset, inputCount, output, 0);
        return output;
    }

    static byte[] CreateState(byte[] key)
    {
        const int StateSize = 256;

        byte[] state = new byte[StateSize];

        for (int i = 0; i < StateSize; ++i)
            state[i] = (byte)i;

        byte index1 = 0;
        byte index2 = 0;
        for (int i = 0; i < StateSize; ++i)
        {
            index2 = (byte)(key[index1] + state[i] + index2);
            SwapElements(state, i, index2);
            index1 = (byte)((index1 + 1) % key.Length);
        }

        return state;
    }

    static void SwapElements(byte[] array, int i, int j)
    {
        byte t = array[i];
        array[i] = array[j];
        array[j] = t;
    }

    static void CheckInputParameters(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        ArgumentNullException.ThrowIfNull(inputBuffer);
        ArgumentOutOfRangeException.ThrowIfNegative(inputOffset);
        ArgumentOutOfRangeException.ThrowIfNegative(inputCount);

        if (inputOffset > inputBuffer.Length - inputCount)
        {
            throw new ArgumentException(
                "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.",
                nameof(inputBuffer));
        }
    }

    int TransformBlockCore(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
    {
        byte[]? state = m_State;
        ObjectDisposedException.ThrowIf(state is null, this);

        for (int i = 0; i < inputCount; ++i)
        {
            ++m_X;
            m_Y = (byte)(state[m_X] + m_Y);
            SwapElements(state, m_X, m_Y);
            byte keyIndex = (byte)(state[m_X] + state[m_Y]);
            byte key = state[keyIndex];

            outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ key);
        }

        return inputCount;
    }
}
