using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Test
{
    static class Snippet
    {
        /// <summary>
        /// Gets snippet stream.
        /// </summary>
        /// <param name="snippetName">The snippet name.</param>
        /// <returns>The snippet stream.</returns>
        public static Stream GetStream(string snippetName)
        {
            if (snippetName == null)
                throw new ArgumentNullException(nameof(snippetName));

            var type = typeof(Snippet);

            var stream = type.Assembly.GetManifestResourceStream(type, "Snippets." + snippetName);
            if (stream == null)
                throw new Exception(string.Format("Snippet '{0}' not found.", snippetName));

            return stream;
        }

        /// <summary>
        /// Gets snippet as a string.
        /// </summary>
        /// <param name="snippetName">The snippet name.</param>
        /// <returns>The snippet content as a string.</returns>
        public static string GetString(string snippetName)
        {
            using (var sr = new StreamReader(GetStream(snippetName)))
                return sr.ReadToEnd();
        }
    }
}
