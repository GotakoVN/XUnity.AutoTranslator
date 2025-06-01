using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace GHT2Standard.XUnityPatcher
{
    public static class TextPatching
    {
      [HarmonyPrefix]
      [HarmonyPatch( typeof( UnityEngine.UI.Text ), "OnEnable" )]
      public static bool TextPrefix( ref UnityEngine.UI.Text __instance )
      {
         if( "HappinessText".Equals( __instance.name ) )
         {
            RectTransform component = __instance.gameObject.GetComponent<RectTransform>();
            if( (double)component.sizeDelta.x == 63.0 )
            {
               component.sizeDelta = new Vector2( 80f, component.sizeDelta.y );
               component.pivot = new Vector2( component.pivot.x, 0.9f );
            }
         }
         return true;
      }
   }
}
