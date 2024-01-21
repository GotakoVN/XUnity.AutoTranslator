using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using XUnity.Common.Extensions;
using XUnity.ResourceRedirector;
using XUnity.AutoTranslator.Plugin.Core.Configuration;
using XUnity.Common.Logging;
using System.Reflection;
using XUnity.AutoTranslator.Plugin.Core;
using System.Collections;
using UnityEngine;

namespace UVPFontProxy.Patcher
{
   public class AssetLoadingPatcher
   {
      private static AssetLoadingPatcher __instance;
      public static void Init()
      {
         __instance = new AssetLoadingPatcher();
         __instance.RegisterHook();
      }

      private void RegisterHook()
      {
         ResourceRedirection.RegisterAssetLoadedHook(
            behaviour: HookBehaviour.OneCallbackPerResourceLoaded,
            action: AssetLoading );
      }

      private void AssetLoading( AssetLoadedContext ctx )
      {
         var asset = ctx.Asset;
         if ( asset != null )
         {
            var uniquePath = ctx.GetUniqueFileSystemAssetPath( asset );
            var type = asset.GetUnityType().FullName;
            XuaLogger.AutoTranslator.Info( $"[UVP] {type} => {uniquePath}" );
            if( "FS2.Client.ClientDataLocalization".Equals( type ) )
            {
               PrintTypeInformation( asset.GetUnityType() );
               try
               {
                  Dictionary<string, string> list = (Dictionary<string, string>)asset.GetUnityType().GetField( "Datas" ).GetValue( asset );
                  XuaLogger.AutoTranslator.Info( $"[UVP] Datas => {list.Count} {list.Keys.Count}" );
                  ((AutoTranslationPlugin)AutoTranslator.Default).TranslateDict( list );
                  asset.GetUnityType().GetField( "Datas" ).SetValue(asset, list );
                  Dictionary<string, string> list2 = (Dictionary<string, string>)asset.GetUnityType().GetField( "Datas" ).GetValue( asset );
               }
               catch ( Exception ex )
               {
                  XuaLogger.AutoTranslator.Error( ex, "Error" );
               }
            }
            else if( "FS2.Settings.GameSettings".Equals( type ) )
            {
               PrintTypeInformation( asset.GetUnityType() );
               var fontSettings = asset.GetUnityType().GetField( "fontSetting" ).GetValue(asset );
               PrintTypeInformation(fontSettings.GetType());
               var damageFontSetting = asset.GetUnityType().GetField( "damageFontSetting" ).GetValue( asset );
               PrintTypeInformation( damageFontSetting.GetType() );
            }
            else if( "FS2.Localizations.LocalizationSetting".Equals( type ) )
            {
               PrintTypeInformation( asset.GetUnityType() );
            }
         }
      }

      private void PrintTypeInformation( Type type )
      {
         try
         {
            foreach( var field in type.GetFields() )
            {
               string privatepublic = field.IsPrivate ? "private" : "public";
               string staticdeclaring = field.IsStatic ? "static" : "";
               XuaLogger.AutoTranslator.Info( $"[FieldInfo] {staticdeclaring} {privatepublic} {field.FieldType.FullName} {field.Name} " );
            }
            foreach( var property in type.GetProperties() )
            {
               XuaLogger.AutoTranslator.Info( $"[PropertyInfo] {property.PropertyType.FullName} {property.Name}" );
            }
         }
         catch( Exception e )
         {

         }
      }
   }
}
