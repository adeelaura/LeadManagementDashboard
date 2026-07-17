using System.Globalization;
using System.Text.RegularExpressions;

namespace LeadManagement.Web.Utilities;

public static partial class ColorUtility
{
    private const string DefaultColor = "#0D6EFD";

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$", RegexOptions.CultureInvariant)]
    private static partial Regex HexColorRegex();

    public static bool IsValidHexColor(string? value) =>
        !string.IsNullOrWhiteSpace(value) && HexColorRegex().IsMatch(value);

    public static string NormalizeOrDefault(string? value)
    {
        if (!IsValidHexColor(value))
        {
            return DefaultColor;
        }

        return value!.ToUpperInvariant();
    }

    public static string GetContrastTextColor(string? backgroundColor)
    {
        var color = NormalizeOrDefault(backgroundColor);
        var red = int.Parse(color.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        var green = int.Parse(color.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        var blue = int.Parse(color.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        // YIQ approximation gives predictable contrast for status badges.
        var yiq = ((red * 299) + (green * 587) + (blue * 114)) / 1000;
        return yiq >= 150 ? "#111827" : "#FFFFFF";
    }
}
