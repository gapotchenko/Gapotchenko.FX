namespace Gapotchenko.FX.IO.Pal;

interface IPalAdapter
{
    bool IsCaseSensitive { get; }

    string GetShortPath(string path);

    string GetRealPath(string path);
}
