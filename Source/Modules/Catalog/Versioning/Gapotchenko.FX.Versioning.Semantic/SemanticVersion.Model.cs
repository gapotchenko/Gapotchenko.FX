// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Versioning.Properties;

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion
{
    /// <summary>
    /// The object model of a semantic version.
    /// </summary>
    readonly struct Model
    {
        /// <summary>
        /// Gets an object model of the specified version string.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <returns>An object model of the specified version string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="version"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="version"/> is empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="version"/> has an invalid format.</exception>
        public static Model Create(string version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (version.Length == 0)
                throw new ArgumentException("Semantic version string cannot be empty.", nameof(version));

            return
                TryParseCore(version) ??
                throw new ArgumentException(Resources.SemanticVersionHasInvalidFormat, nameof(version));
        }

        public int Major { get; init; }
        public int Minor { get; init; }
        public int Patch { get; init; }
        public string? PreReleaseLabel { get; init; }
        public string? BuildLabel { get; init; }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SemanticVersion"/> record using the specified model.
    /// </summary>
    /// <param name="model">The model.</param>
    [return: NotNullIfNotNull(nameof(model))]
    static SemanticVersion? Create(in Model? model) => model is null ? null : new(model.Value);

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> record using the specified model.
    /// </summary>
    /// <param name="model">The model.</param>
    SemanticVersion(in Model model)
    {
        m_Major = model.Major;
        m_Minor = model.Minor;
        m_Patch = model.Patch;
        m_PreReleaseLabel = model.PreReleaseLabel;
        m_BuildLabel = model.BuildLabel;
    }
}
