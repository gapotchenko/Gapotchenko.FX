﻿namespace Gapotchenko.FX.Utilities.MDDocProcessor.Framework
{
    static class Util
    {
        public static string MakeRelativePath(string path, string basePath)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            // Require trailing backslash for path
            if (!Path.EndsInDirectorySeparator(basePath))
                basePath += Path.DirectorySeparatorChar;

            var baseUri = new Uri(basePath);
            var fullUri = new Uri(path);

            var relativeUri = baseUri.MakeRelativeUri(fullUri);

            string relativePath;
            if (relativeUri.IsAbsoluteUri)
            {
                relativePath = relativeUri.LocalPath;
            }
            else
            {
                // Uri's use forward slashes so convert back to backward slashes
                relativePath = Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
    }
}
