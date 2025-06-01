using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using XUnity.Common.Logging;
using XUnity.Common.Utilities;

namespace XUnity.AutoTranslator.Plugin.Core
{
   public class HookNewToolBehaviour :
#if MANAGED
      MonoBehaviour,
#endif
      IMonoBehaviour
   {
      public void OnGUI()
      {
         
      }

      public void Start()
      {
         string managePath = $"{Application.dataPath}/Managed";
         // Load UVPProxy if exist
         string[] proxyDlls = GetDllList( managePath );
         foreach( string dllPath in proxyDlls )
         {
            XuaLogger.AutoTranslator.Info( $"Loading {dllPath}" );
            var proxyDll = LoadAssembly( dllPath );
            if( proxyDll != null )
            {
               XuaLogger.AutoTranslator.Info( $"Processing {dllPath}" );
               try
               {
                  Type[]  types = proxyDll.GetTypes();
                  foreach( var typeItem in types )
                  {
                     XuaLogger.AutoTranslator.Info( $"Processing {typeItem.FullName}" );
                     if (typeItem.IsClass && typeItem.Name.EndsWith("Patch"))
                     {
                        HookingHelper.PatchType( typeItem, false );
                     }
                     else if ( typeItem.IsClass && typeItem.Name.EndsWith( "Patcher" ) )
                     {
                        CallInitMethod( typeItem );
                     }
                  }
               }
               catch( Exception ex )
               {
                  XuaLogger.AutoTranslator.Info( "Error:" + ex.GetType().FullName + ":" + ex.Message );
                  XuaLogger.AutoTranslator.Info( "Error:" + ex.StackTrace );
               }

            }
         }
         /*
          * else if (fileName.Equals( "UnityExplorer.STANDALONE.Mono.dll" ) || fileName.Equals( "UniverseLib.Mono.dll" ) )
            {
               list.Add ( file );
            }
         */
#if MANAGED
         string unityExplorer = $"{managePath}/UnityExplorer.STANDALONE.Mono.dll";
         if( File.Exists( unityExplorer ) )
         {
            try
            {
               var mono = LoadAssembly( $"{managePath}/UniverseLib.Mono.dll" );
               var dll = LoadAssembly( unityExplorer );
               var dllType = dll.GetType( "UnityExplorer.ExplorerStandalone" );
               var instance = System.Activator.CreateInstance( dllType );
               var dllMethod = dllType.GetMethods().FirstOrDefault( m => m.Name == "CreateInstance" && m.GetParameters().Count() == 0 );
               dllMethod.Invoke( instance, null);
            }
            catch( Exception ex )
            {
               XuaLogger.AutoTranslator.Info( $"UnityExplorer.ExplorerStandalone - Error: {ex.Source} {ex.Message} ");
               XuaLogger.AutoTranslator.Info( "UnityExplorer-Error:" + ex.StackTrace );
               if (ex.InnerException != null )
               {
                  XuaLogger.AutoTranslator.Info( $"InnerError: {ex.InnerException.Source} {ex.InnerException.Message} " );
                  XuaLogger.AutoTranslator.Info( "InnerError:" + ex.InnerException.StackTrace );
               }
            }
         }
#endif
      }

      private void CallInitMethod( Type typeItem )
      {
         try
         {
            var initMethod = typeItem.GetMethod( "Init" );
            initMethod.Invoke( null, null );
         }
         catch(Exception ex)
         {
            XuaLogger.AutoTranslator.Warn(ex, $"Cannot call Init() method of {typeItem.FullName}");
         }
      }

      private string[] GetDllList( string managePath )
      {
         List<string> list = new List<string>();
         string[] files = Directory.GetFiles( managePath );
         foreach( string file in files )
         {
            string fileName = Path.GetFileName( file );
            if ( fileName.EndsWith( "XUnityPatcher.dll" ) )
            {
               list.Add( file );
            }
         }
         return list.ToArray();
      }

      public void Update()
      {
         
      }

      private Assembly LoadAssembly( string dllPath)
      {
         // string dllPath = Path.Combine( managePath, dllName ).Replace( "/", "\\" );
         XuaLogger.AutoTranslator.Info( $"Load DLL: {dllPath}" );
         if( File.Exists( dllPath ) )
         {
            try
            {
               XuaLogger.AutoTranslator.Info( "Force loading assembly: " + dllPath );
               var assemblyName = AssemblyName.GetAssemblyName( dllPath.Replace("/","\\" ));
               XuaLogger.AutoTranslator.Info( "AssemblyName: " + assemblyName.FullName );
               var assembly = Assembly.Load( assemblyName );
               if( assembly == null ) throw new Exception( $"{dllPath} return null when loading" );
               else XuaLogger.AutoTranslator.Info( $"{dllPath} loaded OK!" );
               return assembly;
            }
            catch( Exception e )
            {
               XuaLogger.AutoTranslator.Info( e, "An error occurred while force loading assembly: " + e.Message );
            }
         }
         return null;
      }
   }
}
