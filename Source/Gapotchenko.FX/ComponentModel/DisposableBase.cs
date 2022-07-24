namespace Gapotchenko.FX.ComponentModel;

/// <summary>
/// Defines a base class for disposable objects that implement a finalizer pattern.
/// </summary>
public abstract class DisposableBase : IDisposable
{
    /// <summary>
    /// Finalizes the instance of <see cref="Disposable"/> class.
    /// </summary>
    ~DisposableBase()
    {
        Dispose(false);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources;
    /// <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
