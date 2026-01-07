using System.Globalization;

namespace Dashboard.Application.Helpers;

public static class FormattingHelper
{
    public static decimal ParseDecimal(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return 0m;

        // If the input contains a comma, it's likely nl-NL format (comma as decimal separator)
        // Parse with nl-NL culture first to handle cases like "1.234,56" or "1,234"
        if (input.Contains(','))
        {
            if (decimal.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign, CultureInfo.GetCultureInfo("nl-NL"), out var nl))
                return nl;
        }

        // Try parsing with InvariantCulture for standard formats like "123.45"
        if (decimal.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var inv))
        {
            // If it parsed successfully but looks like it might be a thousands separator (e.g., "1.234"),
            // try nl-NL parsing as well to see if it gives a more appropriate result
            // In nl-NL, "1.234" with exactly 3 digits after the dot is typically a thousands separator
            var trimmedInput = input.Trim().TrimStart('+', '-');
            if (trimmedInput.Contains('.') && !trimmedInput.Contains(','))
            {
                var parts = trimmedInput.Split('.');
                // If we have exactly 3 digits after the dot and no more dots, it might be thousands separator
                if (parts.Length == 2 && parts[1].Length == 3)
                {
                    if (decimal.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign, CultureInfo.GetCultureInfo("nl-NL"), out var nlResult))
                    {
                        // Use nl-NL result as it's likely a thousands separator
                        return nlResult;
                    }
                }
            }
            return inv;
        }

        // Final fallback to nl-NL for any remaining cases
        if (decimal.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign, CultureInfo.GetCultureInfo("nl-NL"), out var nlFallback))
            return nlFallback;

        return 0m;
    }

    public static DateOnly ParseDateOnly(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return default;

        if (DateOnly.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        // Use RoundtripKind to properly handle UTC indicators like 'Z'
        // This prevents timezone conversion issues when parsing UTC timestamps
        if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
            return DateOnly.FromDateTime(dt);

        return default; // or throw new FormatException($"Invalid date: {input}");
    }

    public static string FormatDate(DateOnly d) =>
        d == default ? "" : d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string FormatDecimal(decimal d) =>
        d.ToString("0.################", CultureInfo.InvariantCulture);
}