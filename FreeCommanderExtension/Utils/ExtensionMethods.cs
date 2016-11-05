using System.Text.RegularExpressions;

namespace FreeCommanderExtension.Utils
{
    public static class ExtensionMethods
    {
        public static bool ContainsParameter(this string target, string parameter)
        {
            return !string.IsNullOrEmpty(target) &&
                   Regex.IsMatch(target, string.Format(@"(^|\s){0}($|\s)", parameter.Replace("/", @"\/")),
                       RegexOptions.IgnoreCase);
        }
    }
}