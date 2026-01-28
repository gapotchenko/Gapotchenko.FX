// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.AppModel;

/// <summary>
/// Provides information about the app.
/// </summary>
public interface IAppInformation
{
    /// <summary>
    /// Gets the app title.
    /// </summary>
    string? Title { get; }

    /// <summary>
    /// Gets the app description.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the product name.
    /// </summary>
    string? ProductName { get; }

    /// <summary>
    /// Gets the product version.
    /// </summary>
    Version ProductVersion { get; }

    /// <summary>
    /// Gets the product informational version.
    /// </summary>
    string InformationalVersion { get; }

    /// <summary>
    /// Gets the company name.
    /// </summary>
    string? CompanyName { get; }

    /// <summary>
    /// Gets the app copyright information.
    /// </summary>
    string? Copyright { get; }

    /// <summary>
    /// Gets the app trademark information.
    /// </summary>
    string? Trademark { get; }

    /// <summary>
    /// Gets the app executable path.
    /// </summary>
    string ExecutablePath { get; }
}
