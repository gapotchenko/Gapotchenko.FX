using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml
{
    /// <summary>
    /// <see cref="XmlNodeList"/> extensions.
    /// </summary>
    public static class XmlNodeListExtensions
    {
        /// <summary>
        /// Returns a boolean value indicating whether the specified <see cref="XmlNodeList"/> is null or empty.
        /// </summary>
        /// <param name="value">The <see cref="XmlNodeList"/> to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty <see cref="XmlNodeList"/>; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty([NotNullWhen(false)] this XmlNodeList? value) => value is null || value.Count == 0;
    }
}
