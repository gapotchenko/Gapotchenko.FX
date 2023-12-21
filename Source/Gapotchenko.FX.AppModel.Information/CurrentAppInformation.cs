using System.Reflection;

namespace Gapotchenko.FX.AppModel;

sealed class CurrentAppInformation : AppInformation
{
    public static readonly CurrentAppInformation Instance = new();

    protected override Assembly? RetrieveEntryAssembly() => Assembly.GetEntryAssembly();

    protected override Type? RetrieveEntryType() => EntryAssembly?.EntryPoint?.ReflectedType;
}
