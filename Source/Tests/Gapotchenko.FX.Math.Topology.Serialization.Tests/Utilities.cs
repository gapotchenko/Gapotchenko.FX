using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology.Serialization.Tests
{
    static class Utilities
    {
        public static string NormalizeDotDocument(string document)
        {
            var list = new List<string>();

            var reader = new StringReader(document);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    list.Add(line.Trim());
                }
            }

            list.Sort();

            return string.Join(Environment.NewLine, list);
        }
    }
}
