using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using XUnity.Common.Constants;
using XUnity.Common.Harmony;
using static System.Net.Mime.MediaTypeNames;

namespace GHT2.Patcher
{
    public static class TextPatch
    {
      static bool Prepare( object instance )
      {
         return UnityTypes.Text != null;
      }
      static MethodBase TargetMethod( object instance )
      {
         return AccessToolsShim.Method( UnityTypes.Text?.ClrType, "OnEnable" );
      }

      static void Postfix( Component __instance )
      {
         try
         {
            var goProp = UnityTypes.Text_Properties.GameObject;
            var fontSize = UnityTypes.Text_Properties.FontSize;
            var go = goProp.Get( __instance );
            var goName = UnityTypes.GameObject_Properties.Name;
            int size = (int)fontSize.Get( __instance );
            string name = (string)goName.Get( __instance );
            if( "buttonText".Equals( name ) && size == 64 )
            {
               fontSize.Set( __instance, 50 );
            }
         }
         catch( Exception ex )
         {

         }
      }
   }
}
