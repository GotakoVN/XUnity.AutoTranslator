using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace Plugin
{
    public static class FontStore
    {
        private const string DEFAULT_FONT = "Default";
        internal static bool HasFontInOS { get; set; } = false;
        internal static Dictionary<string, Font> fontList = new Dictionary<string, Font>();
        internal static Dictionary<string, Font> fontWithSizeList = new Dictionary<string, Font>();
        internal static Dictionary<string, TMP_FontAsset> fontAssetList = new Dictionary<string, TMP_FontAsset>();
        internal static Dictionary<string, string> config = new Dictionary<string, string>();
        public static TMP_FontAsset MainFontAsset { get; set; } = null;
        public static TMP_FontAsset SubFontAsset { get; set; } = null;
        public static bool HasUpdatedSetting { get; set; } = false;

        private static AssetBundle assetBundle = null;
        public static GameObject FontRoot;
        public static string VN_CHARS = "ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳýỵỷỹqwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789";
        public static void InitRootObject() {
            if (FontRoot == null)
            {
                FontRoot = new GameObject
                {
                    name = "Font Cache"
                };
                FontRoot.SetActive(true);
                UnityEngine.Object.DontDestroyOnLoad(FontRoot);
                Plugin.log.LogMessage("Font Cache is initialized!");
            }
        }
        public static void Init()
        {
            ReadIni();
            Plugin.log.LogMessage("FontStore is initialized!");
            CreateMainSubFontAsset();
        }

        private static void CreateMainSubFontAsset()
        {
            try
            {
                string subFontPath = $"{AssemblyDirectory}\\Font\\LangNhan.ttf";
                if (File.Exists(subFontPath))
                {
                    Font font = new Font(subFontPath);
                    if (font != null)
                    {
                        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font, 60, 6, GlyphRenderMode.SDFAA, 2048, 2048, TMPro.AtlasPopulationMode.Dynamic, true);
                        fontAsset.hideFlags = HideFlags.DontUnloadUnusedAsset;
                        fontAsset.name = "LangNhan";
                        SubFontAsset = fontAsset;
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.log.LogError( ex );
            }
        }

        public static bool InitBundle()
        {
            return false;
        }

        private static TMP_FontAsset CreateFontAsset(Font font, string fontKey)
        {
            try
            {
                TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font, 60, 6, GlyphRenderMode.SDFAA, 2048, 2048, TMPro.AtlasPopulationMode.Dynamic, true);
                if (fontAsset != null)
                {
                    fontAsset.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    fontAsset.name = font.name;
                    foreach (char ch in VN_CHARS.ToCharArray())
                    {
                        fontAsset.HasCharacter(ch, false, true);
                    }
                    TryAddCharacter(fontAsset, (char)12289);
                    TryAddCharacter(fontAsset, (char)65281);
                    TryAddCharacter(fontAsset, (char)65288);
                    TryAddCharacter(fontAsset, (char)65289);
                    TryAddCharacter(fontAsset, (char)65292);
                    TryAddCharacter(fontAsset, (char)65306);
                    TryAddCharacter(fontAsset, (char)65307);
                    TryAddCharacter(fontAsset, (char)65311);
                    TryAddCharacter(fontAsset, (char)65374);
                    TryAddCharacter(fontAsset, (char)65280);
                    TryAddCharacter(fontAsset, (char)65282);
                    TryAddCharacter(fontAsset, (char)12298);
                    TryAddCharacter(fontAsset, (char)12299);
                    TryAddCharacter(fontAsset, (char)12290);
                    TryAddCharacter(fontAsset, (char)12288);
                    
                } 
                else
                {
                   
                }
                return fontAsset;
            }
            catch (Exception ex)
            {
                Plugin.log.LogError( ex );
                return null;
            }
        }
        private static UnityEngine.UI.Text GetTextFromFontRoot(string compName)
        {
            if (FontRoot == null) return null;
            UnityEngine.UI.Text found = null;
            try
            {
                UnityEngine.UI.Text[] texts = FontRoot.GetComponentsInChildren<UnityEngine.UI.Text>();
                if (texts != null && texts.Length > 0)
                {
                    foreach (var text in texts)
                    {
                        if (compName.Equals(text.name))
                        {
                            found = text;
                            break;
                        }
                    }
                }
            } 
            catch(Exception e)
            {
                Plugin.log.LogMessage("--------------Cannot load font UI from root-------------");
                Plugin.log.LogMessage( e );
            }
            return found;
        }

        private static TextMeshProUGUI GetTextFromFontRoot(TextMeshProUGUI[] texts, string compName)
        {
            TextMeshProUGUI found = null;
            if (texts != null && texts.Length > 0)
            {
                foreach (var text in texts)
                {
                    if (compName.Equals(text.name))
                    {
                        found = text;
                        break;
                    }
                }
            }
            return found;
        }

        private static UnityEngine.Font TryGetFont(string fontNameCheck, string compName, int fontSize = 24)
        {
            UnityEngine.Font font = null;
            try
            {
                font = GetUnityFontFromAssetBundle(fontNameCheck);
                if (font != null) return font;
                if (fontWithSizeList.ContainsKey(compName))
                {
                    font = fontWithSizeList[compName];
                }
                if (font == null)
                {
                    font = LoadFontFromOSPath(fontNameCheck, fontSize);
                    if (font == null)
                    {
                        font = GetFontFromDirectory(fontNameCheck);
                    }
                    if (font != null)
                    {
                        if (!fontWithSizeList.ContainsKey(compName) || fontWithSizeList[compName] == null)
                        {
                            fontWithSizeList[compName] = font;
                        }
                    }
                }
                if (font != null)
                {
                    foreach (char ch in VN_CHARS.ToCharArray())
                    {
                        font.HasCharacter(ch);
                    }
                }
            } 
            catch (Exception e)
            {
                Plugin.log.LogError( e );
            }
            return font;
        }

        private static Font GetUnityFontFromAssetBundle(string fontNameCheck)
        {
            string abName = $"{AssemblyDirectory}\\Font\\font";
            if (File.Exists(abName) && assetBundle == null)
            {
                assetBundle = AssetBundle.LoadFromFile(abName);
            }
            if (assetBundle != null)
            {
                return assetBundle.LoadAsset<Font>($"Assets/Font/{fontNameCheck}.ttf");
            }
            return null;
        }

        private static void AddFontToCache(Font font, string compName, UnityEngine.UI.Text text)
        {
            try
            {
                if (FontRoot == null) return;
                if (text == null && FontRoot != null)
                {
                    GameObject gameObject = new GameObject
                    {
                        name = compName
                    };
                    gameObject.SetActive(true);
                    gameObject.transform.parent = FontRoot.transform;
                    text = gameObject.AddComponent<UnityEngine.UI.Text>();
                    text.name = compName;
                }
                if (text != null) text.font = font;
            }
            catch(Exception ex)
            {
                // do nothing
            }
        }

        private static Font GetFontFromDirectory(string fontNameCheck)
        {
            string[] possibleFontPaths = new string[]
                {
                    $"{AssemblyDirectory}\\Font\\{fontNameCheck}.ttf",
                    $"{Paths.GameRootPath}\\{fontNameCheck}.ttf"
                };
            foreach(string possibleFontPath in possibleFontPaths)
            {
                if (File.Exists(possibleFontPath))
                {
                    return new Font(possibleFontPath);
                }
            }
            return null;
        }

        private static TMP_FontAsset TryGetFontAsset(string name, string compName, float fontSize)
        {
            TMP_FontAsset fontAsset = null;
            /*fontAsset = Resources.Load<TMP_FontAsset>(name);
            if (fontAsset != null) return fontAsset;
            fontAsset = GetFromAssetBundle(name);
            if (fontAsset != null) return fontAsset;
            TextMeshProUGUI text = GetTextMeshFromFontRoot(compName);*/
            if (fontAssetList.ContainsKey(compName))
            {
                fontAsset = fontAssetList[compName];
            }
            if (fontAsset == null)
            {
                Font font = TryGetFont(name, compName, (int)fontSize);
                if (font != null)
                {
                    fontAsset = CreateFontAsset(font, compName);
                    if (fontAsset != null && (!fontAssetList.ContainsKey(compName) || fontAssetList[compName] == null))
                    {
                        fontAssetList[compName] = fontAsset;
                    }
                }
            }
            return fontAsset;
        }

        private static void AddTMProToCache(TMP_FontAsset fontAsset, string compName, TextMeshProUGUI text)
        {
            try
            {
                if (FontRoot == null) return;
                if (text == null && FontRoot != null)
                {
                    GameObject gameObject = GetGameObjectFromFontRoot(compName);
                    if (gameObject == null)
                    {
                        gameObject = new GameObject
                        {
                            name = compName
                        };
                        gameObject.SetActive(true);
                        gameObject.transform.parent = FontRoot.transform;
                    }
                    text = gameObject.AddComponent<TextMeshProUGUI>();
                }
                if (text != null)
                {
                    text.name = compName;
                    text.font = fontAsset;
                }
            } 
            catch(Exception ex)
            {
                // do nothing
            }
        }

        private static TMP_FontAsset GetFromAssetBundle(string name)
        {
            string abName = Path.Combine(Paths.GameRootPath, name);
            if (File.Exists(abName))
            {
                var ab = AssetBundle.LoadFromFile(abName);
                return ab.LoadAsset<TMP_FontAsset>(name);
            }
            return null;
        }

        private static TextMeshProUGUI GetTextMeshFromFontRoot(string compName)
        {
            if (FontRoot == null) return null;
            TextMeshProUGUI found = null;
            try
            {
                TextMeshProUGUI[] texts = FontRoot.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts != null && texts.Length > 0)
                {
                    foreach (var text in texts)
                    {
                        if (compName.Equals(text.name))
                        {
                            found = text;
                            break;
                        }
                    }
                }
            } 
            catch (Exception ex)
            {
                Plugin.log.LogMessage("--------------Cannot load font TMPro from root-------------");
                Plugin.log.LogMessage( ex ); 
            }
            return found;
        }

        private static GameObject GetGameObjectFromFontRoot(string compName)
        {
            for(int i = 0; i <FontRoot.transform.childCount;i++)
            {
                GameObject go = FontRoot.transform.GetChild(i).gameObject;
                if (compName.Equals(go.name)) return go;
            }
            return null;
        }

        internal static void TryAddCharacter(TMP_FontAsset fontAsset, char code)
        {
            try
            {
                TMP_Character character = new TMP_Character();
                bool isWorked = fontAsset.HasCharacter(code, false, true);
            } 
            catch(Exception ex)
            {
                Plugin.log.LogError( ex );
            }
        }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        internal static void ReadIni()
        {
            string resourceName = $"{AssemblyDirectory}\\Font\\Font.ini";
            Plugin.log.LogDebug( resourceName );
            if (File.Exists(resourceName))
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(resourceName), Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.Trim().StartsWith("#")) continue;
                        string[] lines = line.Split('=');
                        if (lines.Length != 2)
                        {
                            continue;
                        }
                        config[lines[0].Trim()] = lines[1].Trim();
                    }
                }
            }
        }

        private static Font LoadFontFromOSPath(string fileName, int fontSize)
        {
            Font font = null;
            try
            {
                font = Font.CreateDynamicFontFromOSFont(fileName, fontSize);
                GameObject.DontDestroyOnLoad(font);
            } 
            catch (Exception ex)
            {
                Plugin.log.LogMessage( ex );
            }
            return font;
        }

        private static string GetFontOSPaths(string fontNameCheck)
        {
            try
            {
                string[] fontNames = Font.GetPathsToOSFonts();
                foreach (var fontName in fontNames)
                {
                    if (fontName.EndsWith(fontNameCheck + ".ttf"))
                    {
                        return fontName;
                    }
                }
            } 
            catch (Exception e)
            {
                Plugin.log.LogMessage( e );
            }
            return null;
        }

        private static bool HasFont(string fontNameCheck)
        {
            return GetFontOSPaths(fontNameCheck) != null;
        }

        public static void ReplaceFont(UnityEngine.UI.Text text)
        {
            if (text == null) return;
            Font font = GetOrCreateUnityFont(text.font, text.fontSize, out string fontNameCheck);
            
            if (font != null)
            {
                text.font = font;
            }
            else
            {
            }
        }

        public static Font GetOrCreateUnityFont(Font font, int inputFontSize, out string outputFontName)
        {
            string fontName = null;
            if (font != null)
            {
                fontName = font.name;
            }
            else if (config.Keys.Count > 0)
            {
                fontName = config.First().Key;
            }

            if (fontName == null || !config.ContainsKey(fontName))
            {
                bool replaced = config.Values.Any(value => value.StartsWith(fontName));
                if (!replaced && config.ContainsKey(DEFAULT_FONT))
                {
                    fontName = DEFAULT_FONT;
                }
                else
                {
                    outputFontName = "<<Empty UI Font>>";
                    return null;
                }
            }
            int fontSize = inputFontSize;
            string fontNameAndSize = fontName + "_" + fontSize.ToString();
            string fontNameCheck = config.ContainsKey(fontName) ? config[fontName] : config.First().Value;
            outputFontName = fontNameCheck;
            return TryGetFont(fontNameCheck, fontNameAndSize, fontSize);
        }

        public static void ReplaceFont(TextMeshProUGUI text)
        {
            TMP_FontAsset fontAsset = GetOrCreateTMPFont(text.font, text.fontSize, out string fontNameCheck);
            if (fontAsset != null)
            {
                fontAsset.name = fontNameCheck;
                text.font = fontAsset;
            }
            else
            {
            }
        }

        public static TMP_FontAsset GetOrCreateTMPFont(TMP_FontAsset font, float inputFontSize, out string outputFontName)
        {
            string fontName = null;
            if (font != null)
            {
                fontName = font.name;
            }
            else if (config.Keys.Count > 0)
            {
                fontName = config.First().Key;
            }
            if (fontName == null || !config.ContainsKey(fontName))
            {
                bool replaced = config.Values.Any(value => value.StartsWith(fontName));
                if (!replaced && config.ContainsKey(DEFAULT_FONT))
                {
                    fontName = DEFAULT_FONT;
                }
                else
                {
                    outputFontName = "<<Empty>>";
                    return null;
                }
            }
            int fontSize = (int)inputFontSize;
            string fontNameAndSize = fontName + "_" + fontSize.ToString();
            string fontNameCheck = config.ContainsKey(fontName) ? config[fontName] : config.First().Value;
            outputFontName = fontNameCheck;
            return TryGetFontAsset(fontNameCheck, fontNameCheck, fontSize);
        }

        internal static void TryAddMainSubAsset(TMP_Settings instance)
        {
            try
            {
                if (MainFontAsset != null)
                {
                    var m_defaultFontAssetProp = instance.GetType().GetField("m_defaultFontAsset", BindingFlags.NonPublic | BindingFlags.Instance);
                    m_defaultFontAssetProp.SetValue(instance, MainFontAsset);
                    Plugin.log.LogMessage("Set main font OK");
                }
                if (SubFontAsset != null)
                {
                    List<TMP_FontAsset> fallbackList = new List<TMP_FontAsset>
                    {
                        SubFontAsset
                    };
                    var m_fallbackFontAssetsProp = instance.GetType().GetField("m_fallbackFontAssets", BindingFlags.NonPublic | BindingFlags.Instance);
                    m_fallbackFontAssetsProp.SetValue(instance, fallbackList);
                    Plugin.log.LogMessage("Set fallback font OK");
                }
            }
            catch(Exception e)
            {
                Plugin.log.LogError( e );
            }
        }
    }
}
