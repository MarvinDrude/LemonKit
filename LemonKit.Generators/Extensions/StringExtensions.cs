
namespace LemonKit.Generators.Extensions;

public static class StringExtensions
{

    public static string LowerCapitalize(this string str)
    {

        if(str.Length == 0)
        {
            return str;
        }

        if(str.Length > 512)
        { // faster for larger strings but more allocation
            return char.ToUpperInvariant(str[0]) + str[1..].ToLower();
        }

        Span<char> result = stackalloc char[str.Length];
        ReadOnlySpan<char> source = str.AsSpan();

        result[0] = char.ToUpperInvariant(source[0]);

        for(int e = 1; e < source.Length; e++)
        {
            result[e] = char.ToLowerInvariant(source[e]);
        }

        return result.ToString();

    }

}

