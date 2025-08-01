using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FontStashSharp
{
    public class SystemFonts
    {
        private static readonly IReadOnlyCollection<string> StandardFontLocations;

        private readonly IReadOnlyCollection<string> _searchDirectories;
        private readonly IEnumerable<string> _paths;

        static SystemFonts()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                StandardFontLocations = new[]
                {
                    @"%SYSTEMROOT%\Fonts",
                    @"%APPDATA%\Microsoft\Windows\Fonts",
                    @"%LOCALAPPDATA%\Microsoft\Windows\Fonts",
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                StandardFontLocations = new[]
                {
                    "%HOME%/.fonts/",
                    "%HOME%/.local/share/fonts/",
                    "/usr/local/share/fonts/",
                    "/usr/share/fonts/",
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                StandardFontLocations = new[]
                {
                    // As documented on "Mac OS X: Font locations and their purposes"
                    // https://web.archive.org/web/20191015122508/https://support.apple.com/en-us/HT201722
                    "%HOME%/Library/Fonts/",
                    "/Library/Fonts/",
                    "/System/Library/Fonts/",
                    "/Network/Library/Fonts/",
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Android")))
            {
                StandardFontLocations = new[]
                {
                    "/system/fonts/"
                };
            }
            else
            {
                StandardFontLocations = Array.Empty<string>();
            }
        }

        public SystemFonts()
        {
            string[] expanded = StandardFontLocations.Select(x => Environment.ExpandEnvironmentVariables(x)).ToArray();
            string[] existingDirectories = expanded.Where(x => Directory.Exists(x)).ToArray();

            // We do this to provide a consistent experience with case sensitive file systems.
            _paths = existingDirectories
                .SelectMany(x => Directory.EnumerateFiles(x, "*.*", SearchOption.AllDirectories))
                .Where(x => Path.GetExtension(x).Equals(".ttf", StringComparison.OrdinalIgnoreCase)
                            || Path.GetExtension(x).Equals(".ttc", StringComparison.OrdinalIgnoreCase)
                            || Path.GetExtension(x).Equals(".otf", StringComparison.OrdinalIgnoreCase));

            _searchDirectories = existingDirectories;
        }

        public byte[] GetFontData(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Font name cannot be null or empty.", nameof(fileName));
            }

            foreach (string path in _paths)
            {
                if (Path.GetFileName(path).StartsWith(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        return File.ReadAllBytes(path);
                    }
                    catch
                    {
                        return Array.Empty<byte>();
                    }
                }
            }

            return Array.Empty<byte>();
        }
    }
}