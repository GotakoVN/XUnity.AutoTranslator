using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using XUnity.Common.Constants;
using XUnity.Common.Harmony;
using XUnity.Common.Logging;
using XUnity.Common.MonoMod;
using XUnity.Common.Utilities;

namespace XUnity.AutoTranslator.Plugin.Core.Hooks.Managed
{
   internal static class SuperTextMeshHooks
   {
      public static readonly Type[] All = new[] {
         typeof( SuperTextMesh_OnEnable_Hook ),
         typeof( SuperTextMesh_text_Hook )

      };
   }
   [HookingHelperPriority( HookPriority.Last )]
   internal static class SuperTextMesh_Text_Hook
   {
      static bool Prepare( object instance )
      {
         return UnityTypes.SuperTextMesh != null;
      }

      static MethodBase TargetMethod( object instance )
      {
         return AccessToolsShim.Property( UnityTypes.SuperTextMesh?.ClrType, "Text" )?.GetSetMethod();
      }

      static void _Postfix( Component __instance )
      {
         AutoTranslationPlugin.Current.Hook_TextChanged( __instance, false );
      }

      static void Postfix( Component __instance )
      {
         _Postfix( __instance );
      }

      static Action<Component> _original;

      static void MM_Init( object detour )
      {
         _original = detour.GenerateTrampolineEx<Action<Component>>();
      }

      static void MM_Detour( Component __instance, bool value )
      {
         _original( __instance );
         Postfix( __instance );
      }

   }

   [HookingHelperPriority( HookPriority.Last )]
   internal static class SuperTextMesh_text_Hook
   {
      static bool Prepare( object instance )
      {
         return UnityTypes.SuperTextMesh != null;
      }

      static MethodBase TargetMethod( object instance )
      {
         // CheckType();
         return AccessToolsShim.Property( UnityTypes.SuperTextMesh?.ClrType, "text" )?.GetSetMethod();
      }

      static void _Postfix( Component __instance )
      {
         AutoTranslationPlugin.Current.Hook_TextChanged( __instance, false );
      }

      static void Postfix( Component __instance )
      {
         _Postfix( __instance );
      }

      static Action<Component, string> _original;

      static void MM_Init( object detour )
      {
         _original = detour.GenerateTrampolineEx<Action<Component, string>>();
      }
      static void MM_Detour( Component __instance, string value )
      {
         _original( __instance, value );
         Postfix( __instance );
      }
//      private static void CheckType()
//      {
//         if( UnityTypes.SuperTextMesh == null )
//         {
//            XuaLogger.AutoTranslator.Info( "SuperTextMesh is null" );
//#if MANAGED
//            UnityTypes.SuperTextMesh = UnityTypes.FindType( "SuperTextMesh" );
//#endif
//         }
//      }
   }

   [HookingHelperPriority( HookPriority.Last )]
   internal static class SuperTextMesh_OnEnable_Hook
   {
      static bool Prepare( object instance )
      {
         return UnityTypes.SuperTextMesh != null;
      }

      static MethodBase TargetMethod( object instance )
      {
         //CheckType();
         return AccessToolsShim.Method( UnityTypes.SuperTextMesh?.ClrType, "OnEnable");
      }

//      private static void CheckType()
//      {
//         if (UnityTypes.SuperTextMesh == null)
//         {
//            XuaLogger.AutoTranslator.Info( "SuperTextMesh is null" );
//#if MANAGED
//            UnityTypes.SuperTextMesh = UnityTypes.FindType( "SuperTextMesh" );
//#endif
//         }
//      }

      static void _Postfix( Component __instance)
      {
         AutoTranslationPlugin.Current.Hook_TextChanged( __instance, true );
      }

      static void Postfix( Component __instance )
      {
         _Postfix( __instance );
      }

      static Action<Component> _original;

      static void MM_Init( object detour )
      {
         _original = detour.GenerateTrampolineEx<Action<Component>>();
      }

      static void MM_Detour( Component __instance )
      {
         _original( __instance );
         Postfix( __instance );
      }
   }
}

