namespace Gapotchenko.FX.Utilities.MDDocProcessor.Framework
{
    static class Util
    {
        public static string MakeRelativePath(string path, string basePath)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            // Require trailing backslash for path
            if (!basePath.EndsWith("\\"))
                basePath += "\\";

            Uri baseUri = new Uri(basePath);
            Uri fullUri = new Uri(path);

            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

            if (relativeUri.IsAbsoluteUri)
            {
                return relativeUri.LocalPath;
            }
            else
            {
                // Uri's use forward slashes so convert back to backward slashes
                return Uri.UnescapeDataString(relativeUri.ToString()).Replace("/", "\\");
            }
        }
    }
}
