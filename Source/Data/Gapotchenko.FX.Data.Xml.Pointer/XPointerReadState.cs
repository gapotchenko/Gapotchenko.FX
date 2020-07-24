using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    /// <summary>
    /// Specifies the state of the <see cref="XPointerReader"/>.
    /// </summary>
    public enum XPointerReadState
    {
        /// <summary>
        /// The <see cref="XPointerReader.Read"/> method has not been called yet.
        /// </summary>
        Initial,

        /// <summary>
        /// Reading is in progress.
        /// </summary>
        Interactive,

        /// <summary>
        /// An error occurred that prevents the <see cref="XPointerReader"/> from continuing.
        /// </summary>
        Error,

        /// <summary>
        /// The end of the stream has been reached successfully.
        /// </summary>
        EndOfFile,

        /// <summary>
        /// The <see cref="XPointerReader.Close"/> method has been called and the <see cref="XPointerReader"/> is closed.
        /// </summary>
        Closed
    }
}
