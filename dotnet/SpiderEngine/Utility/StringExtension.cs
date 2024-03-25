namespace SpiderEngine.Utility
{
    using System.Text.RegularExpressions;

    internal static class StringExtension
    {
        public static string FillVariables(this string str, IDictionary<string, string> variables)
        {
            return Regex.Replace(str, @"\$\((.*?)\)", (match) =>
            {
                var key = match.Groups[1].Value;
                return !variables.TryGetValue(key, out var value) ? $"$({key})" : value;
            });
        }
    }
}
