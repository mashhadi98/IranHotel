namespace VisitorApp.Contract.Common;

public static class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var result = new System.Text.StringBuilder();

        foreach (var c in input)
        {
            if (char.IsUpper(c) && result.Length > 0)
            {
                result.Append('_');
            }
            result.Append(char.ToLower(c));
        }

        return result.ToString().ToLower();
    }
}
