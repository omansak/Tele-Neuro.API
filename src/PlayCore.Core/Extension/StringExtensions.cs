using System.Linq;

namespace PlayCore.Core.Extension
{
    public static class StringExtensions
    {
        public static bool IsAllDigit(this string value)
        {
            return value.All(char.IsDigit);
        }
    }
}
