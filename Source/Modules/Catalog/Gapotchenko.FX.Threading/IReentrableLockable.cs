// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Threading;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IReentrableLockable : IRecursiveLockable
{
    void Reenter(int level);
    int ExitAll();
}
