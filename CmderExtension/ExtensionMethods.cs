using System.Text.RegularExpressions;

namespace CmderExtension
{
    internal static class ExtensionMethods
    {
        internal static bool ContainsParameter(this string target, string parameter)
        {
            return !string.IsNullOrEmpty(target) && Regex.IsMatch(target, string.Format(@"(^|\s){0}($|\s)", parameter.Replace("/", @"\/")), RegexOptions.IgnoreCase);
        }
    }
}
