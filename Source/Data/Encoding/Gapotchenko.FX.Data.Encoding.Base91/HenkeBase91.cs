using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides implementation of Jochaim Henke's Base91 (basE91) encoding.
    /// </summary>
    public sealed class HenkeBase91 : GenericHenkeBase91
    {
        private HenkeBase91() :
            base(new TextDataEncodingAlphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&()*+,./:;<=>?@[]^_`{|}~\"", true))
        {
        }
    }
}
