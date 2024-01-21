using System;
using System.Collections.Generic;
using System.Linq;

namespace UVPTranslator
{
    public static class TranslateExtensions
    {
        public static string FirstUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            if (s.First() == '{'
                || s.First() == '<'
                || s.First() == '('
                || s.First() == '"'
                || s.First() == '&') 
                return s;
            return s.First().ToString().ToUpper() + s.Substring(1);
        }

        public static int ToInteger(this string s)
        {
            int result = -1;
            bool b = int.TryParse(s.Trim(), out result);
            return b? result : -1;
        }
    }
}
