using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotDocument
    {
        public DotDocument(DotGraphSyntax root)
        {
            Root = root;
        }

        public DotGraphSyntax Root { get; set; }

        public static DotDocument Load(DotReader reader)
        {
            var parser = new DotParser(reader);
            parser.Parse();
            return new DotDocument(parser.Root);
        }
    }
}
