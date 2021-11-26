﻿namespace Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit
{
    /// <summary>
    /// Classes implementing this interface must supply a
    /// method that merges two location objects to return
    /// a new object of the same type.
    /// GPPG-generated parsers have the default location
    /// action equivalent to "@$ = @1.Merge(@N);" where N
    /// is the right-hand-side length of the production.
    /// </summary>
    /// <typeparam name="TSpan">The Location type</typeparam>
    interface IMerge<TSpan>
    {
        /// <summary>
        /// Creates a location object from the current and last object. 
        /// Typically used to create a location object extending from the 
        /// start of the @1 object to the end of the @N object.
        /// </summary>
        /// <param name="last">The lexically last object to merge.</param>
        /// <returns>The merged location object.</returns>
        TSpan Merge(TSpan last);
    }
}
