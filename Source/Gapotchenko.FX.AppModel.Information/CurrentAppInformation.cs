using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.AppModel
{
    sealed class CurrentAppInformation : AppInformation
    {
        public static readonly CurrentAppInformation Instance = new CurrentAppInformation();

        protected override Assembly GetEntryAssembly() => Assembly.GetEntryAssembly();

        protected override Type GetEntryType() => EntryAssembly?.EntryPoint?.ReflectedType;
    }
}
