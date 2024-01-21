using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UVPTranslator
{
    internal class RegexString
    {
        string pattern;
        string replacement;
        public RegexString(string pattern, string replacement)
        {
            this.pattern = pattern;
            this.replacement = replacement;
        }

        public bool IsMatch(string input)
        {
            return Regex.IsMatch(input, pattern);
        }
        public string Replace(string input)
        {
            return Regex.Replace(input, pattern, replacement);
        }
    }
}
