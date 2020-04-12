using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.AppModel
{
    sealed class DefaultAppInformation : AppInformation
    {
        public static readonly DefaultAppInformation Instance = new DefaultAppInformation();

        protected override Assembly GetEntryAssembly() => Assembly.GetEntryAssembly();

        protected override Type GetEntryType() => EntryAssembly?.EntryPoint?.ReflectedType;
    }
}
