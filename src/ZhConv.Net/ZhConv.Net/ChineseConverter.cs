using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ZhConv.Net;

/// <summary>
/// Provides methods for converting between Simplified and Traditional Chinese characters.
/// </summary>
public static class ChineseConverter
{
    internal static readonly Lazy<Dictionary<string, string>> STCharacters = new(() => LoadCharacters("STCharacters.txt"));
    internal static readonly Lazy<Dictionary<string, string>> TSCharacters = new(() => LoadCharacters("TSCharacters.txt"));

    private static Dictionary<string, string> LoadCharacters(string fileName)
    {
        var lines = ResourceHelper.LoadLinesFromEmbeddedResource(typeof(ChineseConverter).Assembly, fileName);
        return lines.Select(m => m.Split('\t'))
            .Where(m => m.Length == 2)
            .ToDictionary(m => m[0], m => m[1].Split(' ')[0]);
    }

    /// <summary>
    /// Converts Chinese text between Simplified and Traditional forms.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <param name="direction">The conversion direction.</param>
    /// <param name="preferWinApi">If true and running on Windows, uses the Windows API for conversion; otherwise uses the internal character mapping dictionary.</param>
    /// <returns>The converted text.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="direction"/> is not a defined value in <see cref="ChineseConversionDirection"/>.</exception>
    public static string Convert(string text, ChineseConversionDirection direction, bool preferWinApi = false)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        if (Enum.IsDefined(typeof(ChineseConversionDirection), direction) == false)
            throw new ArgumentOutOfRangeException(nameof(direction), direction, "");

        if (text == string.Empty)
            return text;

        return preferWinApi && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? ConvertByWinApi(text, direction)
            : ConvertUsingDictionary(text, direction);
    }

    /// <summary>
    /// Converts Simplified Chinese text to Traditional Chinese.
    /// </summary>
    /// <param name="source">The Simplified Chinese text to convert.</param>
    /// <param name="preferWinApi">If true and running on Windows, uses the Windows API for conversion; otherwise uses the internal character mapping dictionary.</param>
    /// <returns>The text converted to Traditional Chinese.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public static string ToTraditional(string source, bool preferWinApi = false)
    {
        return Convert(source, ChineseConversionDirection.ToTraditional, preferWinApi);
    }

    /// <summary>
    /// Converts Traditional Chinese text to Simplified Chinese.
    /// </summary>
    /// <param name="source">The Traditional Chinese text to convert.</param>
    /// <param name="preferWinApi">If true and running on Windows, uses the Windows API for conversion; otherwise uses the internal character mapping dictionary.</param>
    /// <returns>The text converted to Simplified Chinese.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public static string ToSimplified(string source, bool preferWinApi = false)
    {
        return Convert(source, ChineseConversionDirection.ToSimplified, preferWinApi);
    }

    internal static string ConvertByWinApi(string text, ChineseConversionDirection direction)
    {
        var dwMapFlags = direction == ChineseConversionDirection.ToTraditional
            ? LCMAP_TRADITIONAL_CHINESE
            : LCMAP_SIMPLIFIED_CHINESE;

        var dest = new string(' ', text.Length);
        var code = LCMapStringEx(ZH_CN, dwMapFlags, text, text.Length, dest, dest.Length, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

        if (code == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        return dest;
    }

    internal static string ConvertUsingDictionary(string text, ChineseConversionDirection direction)
    {
        var dic = direction == ChineseConversionDirection.ToTraditional
            ? STCharacters.Value
            : TSCharacters.Value;

        return Replace(text, dic);
    }

    private static string Replace(string text, IReadOnlyDictionary<string, string> characters)
    {
        var changed = false;
        var textElements = text.EnumerateTextElements().ToArray();
        for (var i = 0; i < textElements.Length; i++)
        {
            if (characters.TryGetValue(textElements[i], out var value) == false)
                continue;

            textElements[i] = value;
            changed = true;
        }

        if (changed == false)
            return text;

        return StringBuilderHelper.Build(m =>
        {
            foreach (var element in textElements)
            {
                m.Append(element);
            }
        });
    }

    private const uint LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
    private const uint LCMAP_TRADITIONAL_CHINESE = 0x04000000;
    private const string ZH_CN = "zh-CN";

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int LCMapStringEx(
        string lpLocaleName,
        uint dwMapFlags,
        string lpSrcStr,
        int cchSrc,
        string lpDestStr,
        int cchDest,
        IntPtr lpVersionInformation,
        IntPtr lpReserved,
        IntPtr sortHandle);
}