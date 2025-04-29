using System.Text;

namespace Gapotchenko.FX.Console.Emulation;

sealed class VirtualTerminalWriter : TextWriter
{
    public VirtualTerminalWriter(IVirtualTerminalOutputBackend backend)
    {
        if (backend == null)
            throw new ArgumentNullException(nameof(backend));

        m_Backend = backend;
        m_BackendTextWriter = backend.Out;
    }

    IVirtualTerminalOutputBackend m_Backend;
    TextWriter m_BackendTextWriter;

    public IVirtualTerminalOutputBackend Backend => m_Backend;

    public override Encoding Encoding => m_BackendTextWriter.Encoding;

    public override IFormatProvider FormatProvider => m_BackendTextWriter.FormatProvider;

    enum State
    {
        Default,
        Escape,
        Csi,
        CsiParameterBytes,
        CsiIntermediateBytes
    }

    State m_CurrentState;

    const char EscapeCharacter = '\x1b';

    void _AppendCommandChar(char c)
    {
        (m_CommandBuilder ??= new StringBuilder()).Append(c);
    }

    StringBuilder? m_CommandBuilder;

    public override void Write(string? value)
    {
        _EnterBuffer();
        try
        {
            base.Write(value);
        }
        finally
        {
            _LeaveBuffer();
        }
    }

    public override void WriteLine(string? value)
    {
        _EnterBuffer();
        try
        {
            base.WriteLine(value);
        }
        finally
        {
            _LeaveBuffer();
        }
    }

    public override void WriteLine()
    {
        _EnterBuffer();
        try
        {
            base.WriteLine();
        }
        finally
        {
            _LeaveBuffer();
        }
    }

    [ThreadStatic]
    static int m_BufferEntranceCounter;

    void _EnterBuffer()
    {
        if (m_BufferEntranceCounter == 0)
            _EnterBufferCore();
        ++m_BufferEntranceCounter;
    }

    void _LeaveBuffer()
    {
        --m_BufferEntranceCounter;
        if (m_BufferEntranceCounter == 0)
            _LeaveBufferCore();
    }

    void _EnterBufferCore()
    {
        m_Buffer = new StringBuilder();
    }

    void _LeaveBufferCore()
    {
        _FlushBuffer();
        m_Buffer = null;
    }

    void _WriteCore(char c)
    {
        var buffer = m_Buffer;
        if (buffer == null)
            m_BackendTextWriter.Write(c);
        else
            buffer.Append(c);
    }

    void _FlushBuffer()
    {
        var buffer = m_Buffer;
        if (buffer?.Length > 0)
        {
            m_BackendTextWriter.Write(buffer.ToString());
            buffer.Clear();
        }
    }

    [ThreadStatic]
    static StringBuilder? m_Buffer;

    public override void Write(char c)
    {
        switch (m_CurrentState)
        {
            case State.Escape:
                switch (c)
                {
                    case EscapeCharacter:
                        // Escape an escape character.
                        _WriteCore(c);
                        m_CurrentState = State.Default;
                        break;
                    case '[':
                        m_CurrentState = State.Csi;
                        break;
                    default:
                        // Pass the escape sequence through.
                        _WriteCore(EscapeCharacter);
                        _WriteCore(c);
                        m_CurrentState = State.Default;
                        break;
                }
                break;

            case State.Csi:
            case State.CsiParameterBytes:
                if (c >= 0x30 && c <= 0x3F)
                {
                    _AppendCommandChar(c);
                    m_CurrentState = State.CsiParameterBytes;
                }
                else
                {
                    goto case State.CsiIntermediateBytes;
                }
                break;

            case State.CsiIntermediateBytes:
                if (c >= 0x20 && c <= 0x2F)
                {
                    _AppendCommandChar(c);
                    m_CurrentState = State.CsiIntermediateBytes;
                }
                else if (c >= 0x40 && c <= 0x7E)
                {
                    // Final byte.

                    if (m_CommandBuilder == null)
                        throw new InvalidOperationException();
                    _ExecuteCsi(m_CommandBuilder.ToString(), c);
                    m_CommandBuilder = null;

                    m_CurrentState = State.Default;
                }
                else
                {
                    throw new InvalidDataException("Invalid character in CSI sequence.");
                }
                break;

            case State.Default:
                if (c == EscapeCharacter)
                    m_CurrentState = State.Escape;
                else
                    _WriteCore(c);
                break;

            default:
                throw new InvalidOperationException();
        }
    }

    void _ExecuteCsi(string s, char finalByte)
    {
        switch (finalByte)
        {
            case 'm':
                _SelectGraphicRendition(s);
                break;
        }
    }

    public override void Flush()
    {
        _FlushBuffer();
        m_BackendTextWriter.Flush();
    }

    void _SelectGraphicRendition(string s)
    {
        _FlushBuffer();

        string[] parts = s.Split(';');
        foreach (string part in parts)
        {
            switch (part)
            {
                case "0":
                    m_Backend.ResetColor();
                    break;

                case "30":
                    m_Backend.ForegroundColor = ConsoleColor.Black;
                    break;
                case "31":
                    m_Backend.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case "32":
                    m_Backend.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case "33":
                    m_Backend.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case "34":
                    m_Backend.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case "35":
                    m_Backend.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "36":
                    m_Backend.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case "37":
                    m_Backend.ForegroundColor = ConsoleColor.Gray;
                    break;

                case "39":
                    m_Backend.ResetForegroundColor();
                    break;

                case "40":
                    m_Backend.BackgroundColor = ConsoleColor.Black;
                    break;
                case "41":
                    m_Backend.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case "42":
                    m_Backend.BackgroundColor = ConsoleColor.DarkGreen;
                    break;
                case "43":
                    m_Backend.BackgroundColor = ConsoleColor.DarkYellow;
                    break;
                case "44":
                    m_Backend.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case "45":
                    m_Backend.BackgroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "46":
                    m_Backend.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                case "47":
                    m_Backend.BackgroundColor = ConsoleColor.Gray;
                    break;

                case "49":
                    m_Backend.ResetBackgroundColor();
                    break;

                case "90":
                    m_Backend.ForegroundColor = ConsoleColor.Black;
                    break;
                case "91":
                    m_Backend.ForegroundColor = ConsoleColor.Red;
                    break;
                case "92":
                    m_Backend.ForegroundColor = ConsoleColor.Green;
                    break;
                case "93":
                    m_Backend.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "94":
                    m_Backend.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "95":
                    m_Backend.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case "96":
                    m_Backend.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "97":
                    m_Backend.ForegroundColor = ConsoleColor.White;
                    break;

                case "100":
                    m_Backend.BackgroundColor = ConsoleColor.Black;
                    break;
                case "101":
                    m_Backend.BackgroundColor = ConsoleColor.Red;
                    break;
                case "102":
                    m_Backend.BackgroundColor = ConsoleColor.Green;
                    break;
                case "103":
                    m_Backend.BackgroundColor = ConsoleColor.Yellow;
                    break;
                case "104":
                    m_Backend.BackgroundColor = ConsoleColor.Blue;
                    break;
                case "105":
                    m_Backend.BackgroundColor = ConsoleColor.Magenta;
                    break;
                case "106":
                    m_Backend.BackgroundColor = ConsoleColor.Cyan;
                    break;
                case "107":
                    m_Backend.BackgroundColor = ConsoleColor.White;
                    break;
            }
        }
    }
}
