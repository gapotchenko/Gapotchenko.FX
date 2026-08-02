// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Reflection;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal;

/// <summary>
/// Intrinsic patcher base.
/// </summary>
abstract class Adapter
{
    public virtual bool IsFeatureSupported(MachineCodeIntrinsicFeature feature) => false;

    public abstract PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code);

    public enum PatchResult
    {
        Success,
        UnexpectedPrologue,
        UnsupportedUnwindPrologue,
        InvalidAlignment,
        NoSpace,
        WriteProtected,
        NotSupported
    }
}
