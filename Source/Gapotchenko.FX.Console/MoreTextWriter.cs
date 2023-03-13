using Gapotchenko.FX.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX.Console;

using Console = System.Console;

/// <summary>
/// Automatically manages console behavior when written data exceeds the height of a console area visible to the user.
/// The behavior is very similar to 'more' command line utility.
/// </summary>
/// <remarks>
/// <see cref="MoreTextWriter"/> handles redirected console streams automatically.
/// For those no continue prompts are generated.
/// </remarks>
public class MoreTextWriter : TextWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoreTextWriter"/> class.
    /// </summary>
    public MoreTextWriter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoreTextWriter"/> class.
    /// </summary>
    /// <param name="baseTextWriter">The underlying text writer.</param>
    public MoreTextWriter(TextWriter baseTextWriter) :
        this()
    {
        BaseTextWriter = baseTextWriter;
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic management of console behavior is enabled
    /// when written data exceeds the size of a console area visible to the user.
    /// </summary>
    public bool Enabled { get; set; } = true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_CachedScreenHeight = -1;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int ScreenHeight
    {
        get
        {
            int v = m_CachedScreenHeight;
            if (v == -1)
            {
                v = TryGetScreenHeight() ?? 0;
                m_CachedScreenHeight = v;
            }
            return v;
        }
    }

    [DebuggerHidden]
    static int? TryGetScreenHeight()
    {
        try
        {
            return Console.WindowHeight;
        }
        catch
        {
            return null;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TextWriter? m_BaseTextWriter;

    /// <summary>
    /// Gets or sets the underlying text writer.
    /// </summary>
    public TextWriter BaseTextWriter
    {
        get => m_BaseTextWriter ?? throw new InvalidOperationException("Base text writer is not set.");
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (m_BaseTextWriter == this)
                throw new ArgumentException("Cannot set the base text writer to itself.", nameof(value));

            if (m_BaseTextWriter != value)
            {
                m_BaseTextWriter = value;
                RecalculateSkipCriteria();
            }
        }
    }

    /// <summary>
    /// Discards any information about console state that has been cached.
    /// </summary>
    /// <remarks>
    /// Call this method after <see cref="Console.SetOut(TextWriter)"/> or <see cref="Console.SetError(TextWriter)"/> call.
    /// </remarks>
    public void Refresh()
    {
        m_CachedScreenHeight = -1;
        RecalculateSkipCriteria();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool m_Skip;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool m_SkipCriteriaNeedsProbing;

    void RecalculateSkipCriteria()
    {
        m_SkipCriteriaNeedsProbing = false;
        m_Skip = ScreenHeight == 0;
        if (m_Skip)
            return;

        var baseTextWriter = m_BaseTextWriter;
        if (baseTextWriter == null)
        {
            m_Skip = true;
            return;
        }

        if (!Environment.UserInteractive)
        {
            m_Skip = true;
            return;
        }

        if (baseTextWriter != Console.Error &&
            baseTextWriter != Console.Out)
        {
            m_Skip = true;
            return;
        }

        m_Skip = false;
        m_SkipCriteriaNeedsProbing = true;
    }

    /// <inheritdoc/>
    public override Encoding Encoding => BaseTextWriter.Encoding;

    /// <inheritdoc/>
    public override void Write(char value)
    {
        if (m_Skip || !Enabled)
            BaseTextWriter.Write(value);
        else
            Write(new string(value, 1));
    }

    /// <inheritdoc/>
    public override void Write(char[]? buffer)
    {
        if (m_Skip || !Enabled || buffer == null)
            BaseTextWriter.Write(buffer);
        else
            Write(new string(buffer));
    }

    /// <inheritdoc/>
    public override void Write(char[] buffer, int index, int count)
    {
        if (m_Skip || !Enabled)
            BaseTextWriter.Write(buffer, index, count);
        else
            Write(new string(buffer, index, count));
    }

    /// <inheritdoc/>
    public override void WriteLine(string? value)
    {
        if (!Enabled)
        {
            BaseTextWriter.WriteLine(value);
        }
        else
        {
            Write(value);
            Write(CoreNewLine);
        }
    }

    void WriteCore(string value)
    {
        var baseTextWriter = BaseTextWriter;

        if (!m_SkipCriteriaNeedsProbing || !Enabled)
        {
            baseTextWriter.Write(value);
        }
        else
        {
            // Make a probe to find out whether the output stream is directly rendered to the console or redirected to another destination.

            // If a string is empty or consists only of control characters that MAY NOT affect console canvas then
            // probing is deferred till the next better character sequence is encountered.
            bool canProbeNow =
                !value.EndsWith('\r') &&
                value.AsSpan().Trim(new[] { '\n', '\r', '\b' }).Length != 0;

            if (!canProbeNow)
            {
                baseTextWriter.Write(value);
                return;
            }

            int top = Console.CursorTop;
            int left = Console.CursorLeft;

            baseTextWriter.Write(value);
            baseTextWriter.Flush();

            bool consoleChanged = Console.CursorTop != top || Console.CursorLeft != left;

            m_Skip = !consoleChanged;
            m_SkipCriteriaNeedsProbing = false;
        }
    }

    /// <inheritdoc/>
    public override void Write(string? value)
    {
        var baseTextWriter = BaseTextWriter;

        if (m_Skip || !Enabled || value == null)
        {
            baseTextWriter.Write(value);
            return;
        }

        string[] lines = value.Split('\n');

        int n = lines.Length;
        for (int i = 0; i < n; i++)
        {
            string line = lines[i];

            if (line.Length > 0)
                WriteCore(line);

            if (i != n - 1)
            {
                WriteCore("\n");
                OnNewLine();
            }
        }
    }

    /// <inheritdoc/>
    public override void Flush()
    {
        base.Flush();

        if (m_BaseTextWriter != null)
            m_BaseTextWriter.Flush();
    }

    /// <inheritdoc/>
    public override async Task FlushAsync()
    {
        await base.FlushAsync().ConfigureAwait(false);

        if (m_BaseTextWriter != null)
            await m_BaseTextWriter.FlushAsync().ConfigureAwait(false);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_WrittenLineCount;

    static class CursorRecovery
    {
        static CursorRecovery()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Console.CursorVisible = true;
        }

        public static void Activate()
        {
        }
    }

    static bool _IsCursorVisible()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Console.CursorVisible;
        }
        else
        {
            // Console.CursorVisible retrieval is not available.
            return true;
        }
    }

    void HandleUI()
    {
        var baseTextWriter = BaseTextWriter;

        int left = Console.CursorLeft;
        ShowPrompt(baseTextWriter);
        baseTextWriter.Flush();
        int promptLength = Console.CursorLeft - left;

        bool savedCursorVisible = _IsCursorVisible();
        CursorRecovery.Activate();
        Console.CursorVisible = false;
        try
        {
            for (; ; )
            {
                ConsoleKeyInfo consoleKeyInfo;
                try
                {
                    consoleKeyInfo = Console.ReadKey(true);
                }
                catch (InvalidOperationException)
                {
                    // Input stream is redirected.
                    m_Skip = true;
                    break;
                }

                var action = GetInteractiveAction(consoleKeyInfo.Key);
                switch (action)
                {
                    case InteractiveAction.ScrollToNextPage:
                        m_WrittenLineCount = 0;
                        break;

                    case InteractiveAction.ScrollToNextLine:
                        --m_WrittenLineCount;
                        break;

                    default:
                        continue;
                }

                break;
            }

            baseTextWriter.Write('\r');
            for (int i = 0; i < promptLength; ++i)
                baseTextWriter.Write(' ');
            baseTextWriter.Write('\r');
            baseTextWriter.Flush();
        }
        finally
        {
            Console.CursorVisible = savedCursorVisible;
        }
    }

    /// <summary>
    /// Shows a prompt.
    /// </summary>
    /// <param name="textWriter">The text writer.</param>
    protected virtual void ShowPrompt(TextWriter textWriter)
    {
        textWriter.Write("(Press <Page Down> to scroll page, <Down Arrow> to scroll line)");
    }

    /// <summary>
    /// Defines an interactive action.
    /// </summary>
    protected enum InteractiveAction
    {
        /// <summary>
        /// No action.
        /// </summary>
        None,

        /// <summary>
        /// Scroll to next page.
        /// </summary>
        ScrollToNextPage,

        /// <summary>
        /// Scroll to next line.
        /// </summary>
        ScrollToNextLine
    }

    /// <summary>
    /// Gets an interactive action for the specified console key.
    /// </summary>
    /// <param name="key">The console key.</param>
    /// <returns>The interactive action.</returns>
    protected virtual InteractiveAction GetInteractiveAction(ConsoleKey key) =>
        key switch
        {
            ConsoleKey.PageDown or ConsoleKey.Spacebar => InteractiveAction.ScrollToNextPage,
            ConsoleKey.DownArrow or ConsoleKey.Enter => InteractiveAction.ScrollToNextLine,
            _ => InteractiveAction.None
        };

    void OnNewLine()
    {
        if (++m_WrittenLineCount >= ScreenHeight - 1)
            HandleUI();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (m_BaseTextWriter != null)
                m_BaseTextWriter.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Gets a value indicating whether a <see cref="MoreTextWriter"/> is interactive.
    /// </summary>
    public bool IsInteractive => !m_Skip;
}
