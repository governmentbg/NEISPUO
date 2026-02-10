namespace SB.Common;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text.Unicode;

public static partial class StringUtils
{
    [GeneratedRegex("((?:^|\\s+)\\S+)")]
    private static partial Regex WordRegex();

    // taken from https://mathiasbynens.be/demo/url-regex
    [GeneratedRegex("(https?|ftp):\\/\\/(-\\.)?([^\\s/?\\.#]+\\.?)+(\\/[^\\s]*)?[^\\s\\.]")]
    private static partial Regex UrlRegex();

    public static string JoinNonEmpty(string separator, params string?[] values)
    {
        return string.Join(separator, values.Where(s => !string.IsNullOrEmpty(s)));
    }

    [DbFunction("fn_join_names3", "school_books")]
    public static string JoinNames(string? firstName, string? middleName, string? lastName)
    {
        return JoinNonEmpty(" ", firstName, middleName, lastName);
    }

    [DbFunction("fn_join_names2", "school_books")]
    public static string JoinNames(string? firstName, string? lastName)
    {
        return JoinNonEmpty(" ", firstName, lastName);
    }

    public static string JoinAsNaturalLanguageList(this IEnumerable<string> items)
        => items.ToArray().JoinAsNaturalLanguageList();

    public static string JoinAsNaturalLanguageList(this string[] items)
        => $"{string.Join(", ", items[..^1])} и {items[^1]}";

    public static string? Truncate(this string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }

    public static string? TruncateWithEllipsis(this string? value, int maxLength)
    {
        if (maxLength <= 3)
        {
            throw new ArgumentException("Max length must be greater than 3");
        }

        if (value == null ||
            value.Length <= maxLength)
        {
            return value;
        }

        return value.Truncate(maxLength - 3) + "...";
    }

    public static string TruncateMultilinedTextWithEllipsis(this string value, int maxChars, int maxLines, bool removeEmptyLines)
    {
        if (maxChars < 0)
        {
            throw new Exception($"{nameof(maxChars)} cannot be negative");
        }

        if (maxLines < 0)
        {
            throw new Exception($"{nameof(maxLines)} cannot be negative");
        }

        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (maxChars == 0 || maxLines == 0)
        {
            return "";
        }

        var lines = value.Split(
            new[] { "\r\n", "\n", "\r" },
            removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

        int linesCount = 0;
        var sb = new StringBuilder();
        foreach (var line in lines)
        {
            var wordMatches = WordRegex().Matches(line);
            if (!wordMatches.Any() && removeEmptyLines)
            {
                continue;
            }

            if (maxLines == linesCount)
            {
                // maxLines exceeded
                // terminate with elipsis on a new line
                sb.AppendLine();
                sb.Append("...");
                return sb.ToString();
            }

            foreach (Match wordMatch in wordMatches)
            {
                var word = wordMatch.Value;
                if (sb.Length + word.Length > maxChars)
                {
                    // maxChars exceeded
                    // terminate with elipsis right after the last word
                    sb.Append(" ...");
                    return sb.ToString();
                }

                sb.Append(word);
            }

            linesCount++;
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static readonly HtmlEncoder htmlEncoder = HtmlEncoder.Create(
        allowedRanges: new[]
            {
                UnicodeRanges.BasicLatin,
                UnicodeRanges.Cyrillic,
                UnicodeRanges.CurrencySymbols
            });
    public static string? MakeHtml(this string? value, bool makeAnchorsForUrls, string? anchorClass)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        StringBuilder res = new();

        var paragraphs = value.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        foreach (var paragraph in paragraphs)
        {
            if (string.IsNullOrWhiteSpace(paragraph))
            {
                res.Append($"<p>&nbsp;</p>\n");
            }
            else
            {
                var content = htmlEncoder.Encode(paragraph);
                if (makeAnchorsForUrls)
                {
                    content = UrlRegex().Replace(content, (match) =>
                        {
                            var url = match.Value;
                            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                            {
                                return $"<a href=\"{url}\" target=\"_blank\" {(string.IsNullOrEmpty(anchorClass) ? "" : "class=\"" + anchorClass + "\"")}>{url}</a>";
                            }

                            return url;
                        });
                }

                res.Append($"<p>{content}</p>\n");
            }
        }

        return res.ToString();
    }

    public static string? ToCamelCase(this string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        if (!char.IsUpper(s[0]))
        {
            return s;
        }

        char[] chars = s.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            bool hasNext = i + 1 < chars.Length;
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                break;
            }

            chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
        }

        return new string(chars);
    }
}
