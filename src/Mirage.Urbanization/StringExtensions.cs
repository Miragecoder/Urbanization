using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (string.Empty.Equals(input)) return input;
            if (input.Length == 1) return input.ToUpper();
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
