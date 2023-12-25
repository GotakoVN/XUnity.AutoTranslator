using System;
using System.IO;
using System.Linq;
using UnityEngine;
using XUnity.AutoTranslator.Plugin.Core.Configuration;
using XUnity.AutoTranslator.Plugin.Core.Utilities;
using XUnity.Common.Constants;
using XUnity.Common.Logging;
using XUnity.Common.Utilities;

namespace XUnity.AutoTranslator.Plugin.Core.Fonts
{
   internal static class FontHelper
   {
      public static string VN_CHARS = "ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếệỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳýỵỷỹqwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789";
      public static UnityEngine.Object GetTextMeshProFont( string assetBundle )
      {
         UnityEngine.Object font = null;

         var overrideFontPath = Path.Combine( Paths.GameRoot, assetBundle );
         if( File.Exists( overrideFontPath ) )
         {
            XuaLogger.AutoTranslator.Info( "Attempting to load TextMesh Pro font from asset bundle." );

            AssetBundle bundle = null;
            if( UnityTypes.AssetBundle_Methods.LoadFromFile != null )
            {
               try
               {
                  bundle = (AssetBundle)UnityTypes.AssetBundle_Methods.LoadFromFile.Invoke( null, new object[] { overrideFontPath } );
               }
               finally
               {

               }
            }
            else if( UnityTypes.AssetBundle_Methods.CreateFromFile != null )
            {
               try
               {
                  bundle = (AssetBundle)UnityTypes.AssetBundle_Methods.CreateFromFile.Invoke( null, new object[] { overrideFontPath } );
               }
               finally
               {

               }

            }
            else
            {
               XuaLogger.AutoTranslator.Error( "Could not find an appropriate asset bundle load method while loading font: " + overrideFontPath );
               return null;
            }

            if( bundle == null )
            {
               XuaLogger.AutoTranslator.Warn( "Could not load asset bundle while loading font: " + overrideFontPath );
               return null;
            }

            if( UnityTypes.TMP_FontAsset != null )
            {
               try
               {
                  if( UnityTypes.AssetBundle_Methods.LoadAllAssets != null )
                  {
#if MANAGED
                     var assets = (UnityEngine.Object[])UnityTypes.AssetBundle_Methods.LoadAllAssets.Invoke( bundle, new object[] { UnityTypes.TMP_FontAsset.UnityType } );
#else
                  var assets = (Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object>)UnityTypes.AssetBundle_Methods.LoadAllAssets.Invoke( bundle, new object[] { UnityTypes.TMP_FontAsset.UnityType } );
#endif
                     font = assets?.FirstOrDefault();
                  }
                  else if( UnityTypes.AssetBundle_Methods.LoadAll != null )
                  {
#if MANAGED
                     var assets = (UnityEngine.Object[])UnityTypes.AssetBundle_Methods.LoadAll.Invoke( bundle, new object[] { UnityTypes.TMP_FontAsset.UnityType } );
#else
                  var assets = (Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object>)UnityTypes.AssetBundle_Methods.LoadAll.Invoke( bundle, new object[] { UnityTypes.TMP_FontAsset.UnityType } );
#endif
                     font = assets?.FirstOrDefault();
                  }
               }
               catch( Exception e )
               {
                  XuaLogger.AutoTranslator.Error ("Error when creating font - " + e.Message);
               }
            }
         }
         else
         {
            XuaLogger.AutoTranslator.Info( "Attempting to load TextMesh Pro font from internal Resources API." );
            try
            {
               font = Resources.Load( assetBundle );
            }
            catch ( Exception ex )
            {
               XuaLogger.AutoTranslator.Warn( $"Resource failed. {ex.Message}" );
            }
         }

         if( font != null )
         {
            var versionProperty = UnityTypes.TMP_FontAsset_Properties.Version;
            var version = (string)versionProperty?.Get( font ) ?? "Unknown";
            XuaLogger.AutoTranslator.Info( $"Loaded TextMesh Pro font uses version: {version}" );

            if( versionProperty != null && Settings.TextMeshProVersion != null && version != Settings.TextMeshProVersion )
            {
               XuaLogger.AutoTranslator.Warn( $"TextMesh Pro version mismatch. Font asset version: {version}, TextMesh Pro version: {Settings.TextMeshProVersion}" );
            }

            GameObject.DontDestroyOnLoad( font );
         }
         else
         {
#if MANAGED
            var abPath = Path.Combine( Paths.GameRoot, "font" );
            XuaLogger.AutoTranslator.Info( $"[MANAGED] Attempting to load Text font for TMP Pro from asset bundle {abPath}" );
            if( File.Exists( abPath ) )
            {
               if( bundle == null )
               {
                  if( UnityTypes.AssetBundle_Methods.LoadFromFile != null )
                  {
                     try
                     {
                        bundle = (AssetBundle)UnityTypes.AssetBundle_Methods.LoadFromFile.Invoke( null, new object[] { abPath } );
                     }
                     finally
                     {
                     }
                  }
                  else if( UnityTypes.AssetBundle_Methods.CreateFromFile != null )
                  {
                     try
                     {
                        bundle = (AssetBundle)UnityTypes.AssetBundle_Methods.CreateFromFile.Invoke( null, new object[] { abPath } );
                     }
                     finally
                     {
                     }
                  }
                  else
                  {
                     XuaLogger.AutoTranslator.Error( "Could not find an appropriate asset bundle load method while loading font: " + Settings.OverrideFont );
                  }
               }
               if( bundle != null )
               {
                  try
                  {
                     string abFontPath = "Assets/Font/" + Settings.OverrideFont + ".ttf";
                     XuaLogger.AutoTranslator.Info( $"{abFontPath} from asset bundle {abPath}" );
                     if( UnityTypes.AssetBundle_Methods.LoadAsset != null )
                     {
                        var bundleFont = (UnityEngine.Font)UnityTypes.AssetBundle_Methods.LoadAsset.Invoke( bundle, new object[] { abFontPath, UnityTypes.Font.UnityType } );
                        var methodInfo = UnityTypes.TMP_FontAsset.ClrType.GetMethods()
                                          .Where( m => m.Name == "CreateFontAsset"
                                          && m.IsStatic
                                          && m.IsPublic
                                          && m.GetParameters().Length > 2 ).FirstOrDefault();
                        if( methodInfo == null ) XuaLogger.AutoTranslator.Info( "Sheet method info TMP_FontAsset.CreateFontAsset is null" );
                        var result = methodInfo.Invoke( null, new object[] { bundleFont, 60, 6, EnumHelper.GetValues( UnityTypes.GlyphRenderMode, "SDFAA" ), 2048, 2048, EnumHelper.GetValues( UnityTypes.AtlasPopulationMode, "Dynamic" ), true } );
                        XuaLogger.AutoTranslator.Info( $"{result}" );
                        font = (UnityEngine.Object)result;
                     }
                  }
                  catch( Exception ex )
                  {
                     XuaLogger.AutoTranslator.Error( ex.Message );
                  }
               }
            }
            string ttfPath = overrideFontPath + ".ttf";
            if( File.Exists( ttfPath ) && font == null )
            {
               try
               {
                  XuaLogger.AutoTranslator.Info( $"[MANAGED][TMPro] Create font from {ttfPath}" );
                  var ttfFont = new Font( ttfPath );
                  var methodInfo = UnityTypes.TMP_FontAsset.ClrType.GetMethods()
                     .Where( m => m.Name == "CreateFontAsset"
                     && m.IsStatic
                     && m.IsPublic
                     && m.GetParameters().Length > 2 ).FirstOrDefault();
                  if( methodInfo == null ) XuaLogger.AutoTranslator.Info( "Sheet method info TMP_FontAsset.CreateFontAsset is null" );
                  var result = methodInfo.Invoke( null, new object[] { ttfFont, 60, 6, EnumHelper.GetValues( UnityTypes.GlyphRenderMode, "SDFAA" ), 2048, 2048, EnumHelper.GetValues( UnityTypes.AtlasPopulationMode, "Dynamic" ), true } );
                  XuaLogger.AutoTranslator.Info( $"{result}" );
                  font = (UnityEngine.Object)result;
               }
               catch( Exception ex )
               {
                  XuaLogger.AutoTranslator.Error( ex, "Cannot create TMP_FontAsset from Font" );
               }
            }
#else
            string ttfPath = overrideFontPath + ".ttf";
            if( File.Exists(ttfPath))
            {
               try
               {
                  XuaLogger.AutoTranslator.Info( $"[IL2CPP] Create font for TMP_Pro from {ttfPath}" );
                  Font ttfFont = new Font();
                  Font.Internal_CreateFont(ttfFont, ttfPath);
                  var methodInfo = UnityTypes.TMP_FontAsset.ClrType.GetMethods()
                     .Where( m => m.Name == "CreateFontAsset"
                     && m.IsStatic
                     && m.IsPublic
                     && m.GetParameters().Length > 2 ).FirstOrDefault();
                  if( methodInfo == null ) XuaLogger.AutoTranslator.Info( "Sheet method info TMP_FontAsset.CreateFontAsset is null" );
                  var result = methodInfo.Invoke( null, new object[] { ttfFont, 60, 6, EnumHelper.GetValues( UnityTypes.GlyphRenderMode, "SDFAA" ), 2048, 2048, EnumHelper.GetValues( UnityTypes.AtlasPopulationMode, "Dynamic" ), true } );
                  var hasCharMethodInfo = UnityTypes.TMP_FontAsset.ClrType.GetMethods()
                     .Where( m => m.Name == "HasCharacter"
                     && m.IsPublic
                     && m.GetParameters().Length > 2 ).FirstOrDefault();
                  if( hasCharMethodInfo != null )
                  {
                     XuaLogger.AutoTranslator.Info( "HasCharacter is ready to work" );
                     hasCharMethodInfo.Invoke(result, new object[] {(char)12289, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65281, false,true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65288, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65289, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65292, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65306, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65307, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65311, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65374, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65280, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)65282, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)12298, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)12299, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)12290, false, true});
                     hasCharMethodInfo.Invoke(result, new object[] {(char)12288, false, true});
                     foreach(char ch in VN_CHARS.ToCharArray())
                     {
                        hasCharMethodInfo.Invoke(result, new object[] {ch, false, true});
                     }
                     
                  }
                  font = (UnityEngine.Object)result;
               }
               catch(Exception ex)
               {
                  XuaLogger.AutoTranslator.Error( ex, $"Cannot create TMP_FontAsset from Font {ttfPath}");
               }
            }
#endif
            if( font == null )
            {
               XuaLogger.AutoTranslator.Error( "Could not find the TextMeshPro font asset: " + assetBundle );
            }
         }

         return font;
      }

#if IL2CPP
      public static UnityEngine.Object GetTextMeshProFontByCustomProxies( string assetBundle )
      {
         UnityEngine.Object font = null;

         var overrideFontPath = Path.Combine( Paths.GameRoot, assetBundle );
         if( File.Exists( overrideFontPath ) )
         {
            XuaLogger.AutoTranslator.Info( "Attempting to load TextMesh Pro font from asset bundle." );
            
            var bundle = AssetBundleProxy.LoadFromFile( overrideFontPath );

            if( bundle == null )
            {
               XuaLogger.AutoTranslator.Warn( "Could not load asset bundle while loading font: " + overrideFontPath );
               return null;
            }

            if( UnityTypes.TMP_FontAsset != null )
            {
               var assets = bundle.LoadAllAssets( UnityTypes.TMP_FontAsset.UnityType );
               font = assets?.FirstOrDefault();
            }
         }

         if( font != null )
         {
            var versionProperty = UnityTypes.TMP_FontAsset_Properties.Version;
            var version = (string)versionProperty?.Get( font ) ?? "Unknown";
            XuaLogger.AutoTranslator.Info( $"Loaded TextMesh Pro font uses version: {version}" );

            if( versionProperty != null && Settings.TextMeshProVersion != null && version != Settings.TextMeshProVersion )
            {
               XuaLogger.AutoTranslator.Warn( $"TextMesh Pro version mismatch. Font asset version: {version}, TextMesh Pro version: {Settings.TextMeshProVersion}" );
            }

            GameObject.DontDestroyOnLoad( font );
         }
         else
         {
            XuaLogger.AutoTranslator.Error( "Could not find the TextMeshPro font asset: " + assetBundle );
         }

         return font;
      }
#endif

      static AssetBundle bundle = null;
      public static Font GetTextFont( int size )
      {
         Font font = null;
         var abPath = Path.Combine( Paths.GameRoot, "font" );
         if( File.Exists( abPath ) )
         {
            XuaLogger.AutoTranslator.Info( $"[Text] Attempting to load Text font from asset bundle {abPath}" );
            if( bundle == null )
            {
               if( UnityTypes.AssetBundle_Methods.LoadFromFile != null )
               {
                  try
                  {
                     bundle = (AssetBundle)UnityTypes.AssetBundle_Methods.LoadFromFile.Invoke( null, new object[] { abPath } );
                  }
                  catch(Exception )
                  {
                     XuaLogger.AutoTranslator.Warn( "Cannot load bundle using LoadFromFile" );
                  }
                  finally
                  {
                  }
               }
               else if( UnityTypes.AssetBundle_Methods.CreateFromFile != null )
               {
                  try
                  {
                     bundle = (AssetBundle)UnityTypes.AssetBundle_Methods.CreateFromFile.Invoke( null, new object[] { abPath } );
                  }
                  catch( Exception )
                  {
                     XuaLogger.AutoTranslator.Warn( "Cannot load bundle using CreateFromFile" );
                  }
                  finally
                  {
                  }
               }
               else
               {
                  XuaLogger.AutoTranslator.Error( "Could not find an appropriate asset bundle load method while loading font: " + Settings.OverrideFont );
               }
            }
            if( bundle != null )
            {
               try
               {
                  string abFontPath = "Assets/Font/" + Settings.OverrideFont + ".ttf";
                  XuaLogger.AutoTranslator.Info( $"{abFontPath} from asset bundle {abPath}" );
                  if( UnityTypes.AssetBundle_Methods.LoadAsset != null )
                  {
#if MANAGED
                     font = (UnityEngine.Font)UnityTypes.AssetBundle_Methods.LoadAsset.Invoke( bundle, new object[] { abFontPath, UnityTypes.Font.UnityType } );
#else
                     var asset = (UnityEngine.Object)UnityTypes.AssetBundle_Methods.LoadAsset.Invoke( bundle, new object[] { abFontPath, UnityTypes.Font.UnityType } );
                     font = asset.Cast<UnityEngine.Font>();
#endif
                  }
               }
               catch( Exception ex )
               {
                  XuaLogger.AutoTranslator.Error( ex.Message );
               }
            }
         }
         if( font == null )
         {
            var fontPath = Path.Combine( Paths.GameRoot, Settings.OverrideFont ) + ".ttf";
            XuaLogger.AutoTranslator.Info( $"[TEXT] Create font from {fontPath}" );
            if( File.Exists( fontPath ) )
            {
#if MANAGED
               XuaLogger.AutoTranslator.Info( $"Creating {fontPath}" );
               font = new Font( fontPath );
               font = Font.CreateDynamicFontFromOSFont( fontPath, size );
#else
               if (font == null)
               {
                  try
                  {
                     // font = new Font();
                     font = Font.CreateDynamicFontFromOSFont("Arial", size);
                     Font.Internal_CreateFont(font, fontPath);
                  }
                  catch(Exception ex)
                  {
                     // do nothing
                  }
               }
               
#endif
            }
            else
            {
               try
               {
                  font = Font.CreateDynamicFontFromOSFont( Settings.OverrideFont, size );
               }
               catch( Exception ex )
               {
                  XuaLogger.AutoTranslator.Warn( $"Cannot create font {Settings.OverrideFont}" );
               }
            }
            if( font != null ) GameObject.DontDestroyOnLoad( font );
         }
         return font;
      }

      public static string[] GetOSInstalledFontNames()
      {
         return Font.GetOSInstalledFontNames();
      }
   }
}
