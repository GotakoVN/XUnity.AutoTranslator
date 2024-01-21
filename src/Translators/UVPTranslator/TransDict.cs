using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace UVPTranslator
{
    internal static class TransDict
    {
        // UIText
        public static Dictionary<string, string> UIText = new Dictionary<string, string>();
        public static Dictionary<string, string> UITextMesh = new Dictionary<string, string>();
        public static Dictionary<string, string> UIIMGUI = new Dictionary<string, string>();
        public static Dictionary<string, string> DllText = new Dictionary<string, string>();
        public static Dictionary<string, string> TextAsset = new Dictionary<string, string>();
        public static List<RegexString> RegexText = new List<RegexString>();
        public static Dictionary<string, string> FailedText = new Dictionary<string, string>();
        public static Dictionary<string, string> BigTexts = new Dictionary<string, string>();
        public static long UITextCount = 0;
        public static long UITextMeshCount = 0;
        public static long FailedTextCount = 0;
        public static long DllTextCount = 0;
        public static string CurrentDir = null;
        internal static void Initialize(string translatorDir)
        {
            CurrentDir = translatorDir;
            LoadDicts();
            CleanLackLogs();

        }

        internal static void LoadDicts()
        {
            UIText = LoadFileToDictionary("Text");
            UITextCount = UIText.Count;
            UITextMesh = LoadFileToDictionary("TextMesh");
            UITextMeshCount = UITextMesh.Count;
            DllText = LoadFileToDictionary("DllText");
            DllTextCount = DllText.Count;
            BuildRegexText(TextAsset);
        }

        private static void BuildRegexText(Dictionary<string, string> dict)
        {
            foreach (var kv in dict)
            {
                string key = kv.Key;
                string value = kv.Value;
                if (key.IndexOf("{0}") >= 0 && value.IndexOf("{0}") >= 0)
                {
                    string pattern = new StringBuilder(key)
                        .Replace("(", @"\(")
                        .Replace(")", @"\)")
                        .Replace("[", @"\[")
                        .Replace("]", @"\]")
                        .Replace("*", @"\*")
                        .Replace("?", @"\?")
                        .Replace("|", @"\|")
                        .Replace("+", @"\+")
                        .Replace("^", @"\^")
                        .Replace("$", @"\$")
                        .Replace("{0}", "(.+)")
                        .Replace("{1}", "(.+)")
                        .Replace("{2}", "(.+)")
                        .Replace("{3}", "(.+)")
                        .Replace("{4}", "(.+)")
                        .Replace("{5}", "(.+)")
                        .Replace("{6}", "(.+)")
                        .Replace("{7}", "(.+)")
                        .Replace("{8}", "(.+)")
                        .Replace("{9}", "(.+)")
                        .ToString();
                    string replacement = new StringBuilder(key)
                        .Replace("{0}", "$1")
                        .Replace("{1}", "$2")
                        .Replace("{2}", "$3")
                        .Replace("{3}", "$4")
                        .Replace("{4}", "$5")
                        .Replace("{5}", "$6")
                        .Replace("{6}", "$7")
                        .Replace("{7}", "$8")
                        .Replace("{8}", "$9")
                        .Replace("{9}", "$10")
                        .ToString();
                    RegexText.Add(new RegexString(pattern, replacement));
                }
            }
        }

        private static Dictionary<string, string> LoadExtraTexts()
        {
            string txtPath = Path.Combine(CurrentDir, "UVPTranslator");

            Dictionary<string, string> dics = new Dictionary<string, string>();
            string[] files = Directory.GetFiles(txtPath, "*.ini");
            var newDics = LoadExtraIniTexts(files);
            foreach (var keyValue in newDics)
            {
                dics[keyValue.Key] = keyValue.Value;
            }
            string[] txtFiles = Directory.GetFiles(txtPath, "*.txt");
            var txtNewDics = LoadExtraIniTexts(txtFiles);
            foreach (var keyValue in txtNewDics)
            {
                dics[keyValue.Key] = keyValue.Value;
            }
            return dics;
        }

        private static Dictionary<string, string> LoadExtraIniTexts(string[] files)
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            foreach (string file in files)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        string line = null;
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            if (line.Length == 0) continue;
                            int index = 0;
                            while (index < line.Length)
                            {
                                char c = line[index];
                                if ((c == '=' || c == '¤')
                                        && index > 0
                                        && line[index - 1] != '\\'
                                        && (index < line.Length - 1))
                                {
                                    string key = line.Substring(0, index);
                                    dics[key] = line.Substring(index + 1);
                                    break;
                                }
                                index++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO
                }
            }
            return dics;
        }

        internal static string PatchAndLogToDict(string text, string dict, Dictionary<String, String> dictObject)
        {
            return PatchText(text, dict, dictObject, (t, d) => FailedText[t] = d);
        }

        private static string StripFileName(string fileName)
        {
            if (fileName.Contains("/")) return fileName.Substring(fileName.IndexOf("/") + 1);
            return fileName;
        }

        internal static void WriteLackTextsToFile(bool forceSave = false)
        {
            WriteDict("Text", UIText, ref UITextCount);
            WriteDict("TextMesh", UITextMesh, ref UITextMeshCount);
            WriteDict("DllText", DllText, ref DllTextCount);
            if (FailedText.Count > FailedTextCount)
            {
                WriteDictToFile("FailedText", FailedText);
                FailedTextCount = FailedText.Count;
            }
        }
        internal static void WriteDict(string dictName, Dictionary<string, string> dict, ref long count)
        {
            int currentCount = dict.Count;
            foreach (var dictObject in LoadFileToDictionary(dictName))
            {
                if (!dict.ContainsKey(dictObject.Key)
                    || !dict[dictObject.Key].Equals(dictObject.Value))
                {
                    dict[dictObject.Key] = dictObject.Value;
                }
            }
            if (dict.Count > count || currentCount != count)
            {
                WriteDictToFile(dictName, dict);
                count = dict.Count;
            }
        }
        internal static void WriteDictToFile(string file, Dictionary<string, string> dict)
        {
            string txtPath = Utils.PathCombine(CurrentDir, "UVPTranslator", file + ".json");

            File.WriteAllText(txtPath, JsonConvert.SerializeObject(dict, Formatting.Indented), Encoding.UTF8);
        }

        private static void CleanLackLogs()
        {
            string txtPath = Utils.PathCombine(CurrentDir, "UVPTranslator", "FailedText.json");
            if (File.Exists(txtPath)) File.Delete(txtPath);
        }

        internal static Dictionary<string, string> LoadFileToDictionary(string fileName, char separator = '¤')
        {
            string txtPath = Utils.PathCombine(CurrentDir, "UVPTranslator", fileName + ".json");
            Dictionary<string, string> dics = new Dictionary<string, string>();
            if (File.Exists(txtPath))
            {
                try
                {
                    dics = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(txtPath, Encoding.UTF8));
                }
                catch (Exception ex)
                {
                    // TODO
                }
            }
            return dics;
        }

        public static IEnumerable<string> ReadLines(string content)
        {
            using (var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(content))))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        internal static string PatchText(string input, string dictName, Dictionary<string, string> dist, Action<string, string> action)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string result = input;
            if (dist.ContainsKey(input))
            {
                result = dist[input];
            }
            else  // use RegEx and DLLText
            {
                result = CheckRegExText(result, out bool isReplace);
                if (!isReplace)
                {
                    if (TranslateCenter.CanWork)
                    {
                        result = TranslateCenter.Translate(input, trim: true);
                        if (!Utils.IsChinese(result)) dist[input] = result;
                    }
                    action.Invoke(input, dictName);
                }
            }
            return result;
        }

        private static string CheckRegExText(string input, out bool isReplace)
        {
            // use RegEx
            foreach (RegexString regexString in RegexText)
            {
                if (regexString.IsMatch(input))
                {
                    isReplace = true;
                    return regexString.Replace(input);
                }
            }
            isReplace = false;
            return input;
        }

        internal static string LookUpText(string lookupText)
        {
            string result = lookupText;
            if (UIText.ContainsKey(lookupText))
            {
                result = UIText[lookupText];
            }
            else if (UITextMesh.ContainsKey(lookupText))
            {
                result = UITextMesh[lookupText];
            }
            else if (TextAsset.ContainsKey(lookupText))
            {
                result = TextAsset[lookupText];
            }
            return result;
        }
        internal static string Patch(string text)
        {
            string result = LookUpText(text);
            bool patched = !result.Equals(text);
            if (!patched)
            {
                result = PatchAndLogDllText(text);
            }
            return result;
        }

        internal static string PatchAndLogDllText(string text)
        {
            string result = text;
            var matches = Utils.CJK_REGEX_GRP.Matches(text);
            if (matches.Count > 0)
            {
                List<string> sortedList = new List<string>();
                foreach (Match match in matches) sortedList.Add(match.Value);
                sortedList.Sort((x, y) => {
                    if (x.Length < y.Length) return 1;
                    else if (x.Length > y.Length) return -1;
                    else return 0;
                }
                );
                StringBuilder builder = new StringBuilder(text);
                foreach (string str in sortedList)
                {
                    if (DllText.ContainsKey(str))
                    {
                        builder.Replace(str, DllText[str]);
                    }
                    else
                    {
                        FailedText[str] = "DllText";
                        if (TranslateCenter.CanWork)
                        {
                            string translatedStr = TranslateCenter.Translate(str, prefix: "", postfix: " ", true);
                            builder.Replace(str, translatedStr);
                            DllText[str] = translatedStr;
                        }
                    }
                }
                builder.Replace("  ", " ");
                result = CleanUpAndDoUpperCaseForRows(builder.ToString().Trim());
            }
            return result;
        }


        private static string PatchDllText(string text, out bool patched)
        {
            patched = false;
            string result = text;
            var matches = Utils.CJK_REGEX_GRP.Matches(text);
            if (matches.Count > 0)
            {
                List<string> sortedList = new List<string>();
                foreach (Match match in matches) sortedList.Add(match.Value);
                sortedList.Sort((x, y) => {
                    if (x.Length < y.Length) return 1;
                    else if (x.Length > y.Length) return -1;
                    else return 0;
                }
                );
                StringBuilder builder = new StringBuilder(text);
                foreach (string str in sortedList)
                {
                    if (DllText.ContainsKey(str))
                    {
                        ReplaceTextInBuilder(builder, str, DllText);
                    }
                }
                result = CleanUpAndDoUpperCaseForRows(builder.ToString());

                if (!Utils.IsChinese(result))
                {
                    patched = true;
                }
            }
            return result;
        }
        private static string CleanUpAndDoUpperCaseForRows(string result)
        {
            if (result == null || result.Length == 0) return result;
            char[] chars = result.ToCharArray();
            chars[0] = chars[0].ToString().ToUpper()[0];
            bool isControl = false;
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                if (ch == '\n') { isControl = true; continue; }
                if (isControl)
                {
                    chars[i] = chars[i].ToString().ToUpper()[0];
                    isControl = false;
                }
            }
            return new string(chars);
        }
        private static void ReplaceTextInBuilder(StringBuilder builder, string str, Dictionary<string, string> dllText)
        {
            string strValue = dllText[str];
            builder.Replace(str, strValue);
        }
    }
}
