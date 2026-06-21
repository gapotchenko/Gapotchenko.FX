// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if !HAS_TARGET_PLATFORM || LINUX

namespace Gapotchenko.FX.Security.Cryptography.Pal.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
sealed class PalAdapter : IPalAdapter
{
    public static PalAdapter Instance { get; } = new();

    public bool? QueryFipsPolicy()
    {
        string policyFilePath = "/proc/sys/crypto/fips_enabled";
        if (File.Exists(policyFilePath))
        {
            // Used at least in:
            //   - Red Hat Enterprise Linux

            using var stream = File.OpenRead(policyFilePath);
            if (stream.Length <= 16)
            {
                var reader = new StreamReader(stream);
                var value = reader.ReadToEnd().AsSpan().Trim();

                if (value.Equals("0", StringComparison.Ordinal))
                    return false;
                else if (value.Equals("1", StringComparison.Ordinal))
                    return true;
            }
        }

        return null;
    }
}

#endif
