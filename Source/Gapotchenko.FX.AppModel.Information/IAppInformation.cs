using System;

#nullable enable

namespace Gapotchenko.FX.AppModel
{
    /// <summary>
    /// App information interface.
    /// </summary>
    public interface IAppInformation
    {
        /// <summary>
        /// Gets app title.
        /// </summary>
        string? Title { get; }

        /// <summary>
        /// Gets app description.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets product name.
        /// </summary>
        string? ProductName { get; }

        /// <summary>
        /// Gets product version.
        /// </summary>
        Version ProductVersion { get; }

        /// <summary>
        /// Gets product informational version.
        /// </summary>
        string InformationalVersion { get; }

        /// <summary>
        /// Gets company name.
        /// </summary>
        string? CompanyName { get; }

        /// <summary>
        /// Gets app copyright information.
        /// </summary>
        string? Copyright { get; }

        /// <summary>
        /// Gets app trademark information.
        /// </summary>
        string? Trademark { get; }

        /// <summary>
        /// Gets app executable path.
        /// </summary>
        string ExecutablePath { get; }
    }
}
