// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Reflection;

namespace Gapotchenko.FX.AppModel;

sealed class CurrentAppInformation : AppInformation
{
    public static readonly CurrentAppInformation Instance = new();

    protected override Assembly? RetrieveEntryAssembly() => Assembly.GetEntryAssembly();

    protected override Type? RetrieveEntryType() => EntryAssembly?.EntryPoint?.ReflectedType;
}
