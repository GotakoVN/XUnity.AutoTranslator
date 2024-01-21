using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace UVPTranslator
{
    internal static class Utils
    {
        //【Cùng】
        public static string StripChineseChars(string input)
        {
            StringBuilder result = new StringBuilder(input);
            result = result.Replace('\uff0c', ',')
                           .Replace('\uff01', '!')
                           .Replace('\uff08', '(')
                           .Replace('\uff09', ')')
                           .Replace('\uff1a', ':')
                           .Replace('\uff1b', ';')
                           .Replace('\uff1f', '?')
                           .Replace('\uff5e', '~')
                           .Replace('\u2026', '.')
                           //.Replace('\u201c', '"')
                           //.Replace('\u201d', '"')
                           .Replace('—', '-')
                           .Replace('鐾', ' ')
                           .Replace('\u203b', '*')
                           .Replace('\u3000', ' ')
                           .Replace('\u3001', ',')
                           .Replace('\u3002', '.')
                           .Replace('\u300a', '(')
                           .Replace('\u300b', ')')
                           .Replace('\u3010','[')
                           .Replace('\u3011', ']');
            return result.ToString();
        }
        //public static bool IsChinese(string text)
        //{
        //    return text != null && (IsChineseA(text) || IsChineseB(text));
        //}
        public static bool IsChineseA(string text)
        {
            return text.Any(c => c >= 0x2000 && c <= 0xFA2D);
        }

        private static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static readonly Regex CJK_REGEX_GRP = new Regex(@"\p{IsCJKUnifiedIdeographs}+");

        public static readonly Regex vnRegex = new Regex(@"[ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ]+");
        public static bool IsChinese(this string c)
        {
            return cjkCharRegex.IsMatch(c);
        }

        public static bool IsVietnamese(this string c)
        {
            return vnRegex.IsMatch(c);
        }
        public static string PathCombine(string source, params string[] others)
        {
            string path = source;
            foreach(string other in others)
            {
                path = Path.Combine(path, other);
            }
            return path;
        }
        //public static string ReplaceText(this string c, params (string key, string value)[] values)
        //{
        //    var sb = new StringBuilder(c);
        //    foreach(var v in values)
        //    {
        //        sb = sb.Replace(v.key, v.value);
        //    }
        //    return sb.ToString();
        //}
        public static string Replace(this Match match, string source, string replacement)
        {
            return source.Substring(0, match.Index) + replacement + source.Substring(match.Index + match.Length);
        }

        internal static int ChineseCharCount(string chText)
        {
            return cjkCharRegex.Matches(chText).Count;
        }
    }
}
