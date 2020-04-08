using System;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.InteropServices
{
    /// <summary>
    /// Indicates that the attributed method is exported by the dynamic-link library (DLL) as a static entry point.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DllExportAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DllExportAttribute"/> class with the attributed method's name as the name of entry point to export.
        /// </summary>
        public DllExportAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DllExportAttribute"/> class with the specified entry point to export.
        /// </summary>
        /// <param name="entryPoint">The name or ordinal of the DLL entry point to export.</param>
        public DllExportAttribute(string entryPoint)
        {
            EntryPoint = entryPoint;
        }

        /// <summary>
        /// Gets the name or ordinal of the DLL entry point to export.
        /// </summary>
        public string EntryPoint { get; }

        /// <summary>
        /// Indicates the calling convention of the entry point.
        /// </summary>
        public CallingConvention CallingConvention { get; set; }
    }
}
