﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gapotchenko.FX.IO.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Gapotchenko.FX.IO.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Access to the path &apos;{0}&apos; is denied..
        /// </summary>
        internal static string AccessToPathXDenied {
            get {
                return ResourceManager.GetString("AccessToPathXDenied", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Non-negative number required..
        /// </summary>
        internal static string ArgumentOutOfRange_NonNegativeNumberRequired {
            get {
                return ResourceManager.GetString("ArgumentOutOfRange_NonNegativeNumberRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Must not be greater than the length of the buffer..
        /// </summary>
        internal static string ArgumentOutOfRange_NotGreaterThanBufferLength {
            get {
                return ResourceManager.GetString("ArgumentOutOfRange_NotGreaterThanBufferLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find file or directory &apos;{0}&apos;. File system entry does not exist..
        /// </summary>
        internal static string FileSystemEntryXDoesNotExsit {
            get {
                return ResourceManager.GetString("FileSystemEntryXDoesNotExsit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path &apos;{0}&apos; points to a directory instead of file and thus cannot be enlisted in the file transaction..
        /// </summary>
        internal static string PathPointsToDirectoryNotFileTX {
            get {
                return ResourceManager.GetString("PathPointsToDirectoryNotFileTX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path &apos;{0}&apos; points to a file instead of directory and thus cannot be enlisted in the directory transaction..
        /// </summary>
        internal static string PathPointsToFileNotDirectoryTX {
            get {
                return ResourceManager.GetString("PathPointsToFileNotDirectoryTX", resourceCulture);
            }
        }
    }
}
