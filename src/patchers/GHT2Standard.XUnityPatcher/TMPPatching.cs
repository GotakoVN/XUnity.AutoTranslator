using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

namespace GHT2Standard.XUnityPatcher
{
    public static class TMPPatching
    {
      [HarmonyPrefix]
      [HarmonyPatch( typeof( TextMeshProUGUI ), "OnEnable" )]
      public static bool TextMeshProUGUIPrefix( ref TextMeshProUGUI __instance )
      {
         if( "text".Equals( __instance.name ) && __instance.gameObject.transform.parent != null)
         {
            var goParent = __instance.gameObject.transform.parent.gameObject;
            if( "maintaskname".Equals( goParent.name ) )
            {
               RectTransform component = __instance.gameObject.GetComponent<RectTransform>();
               if( (double)component.localPosition.x >= 476 && (double)component.localPosition.x <490)
               {
                  component.localPosition = new Vector2( 490, component.localPosition.y );
               }
            }
         }
         else if( "title".Equals( __instance.name ) && __instance.gameObject.transform.parent != null )
         {
            var goParent = __instance.gameObject.transform.parent.gameObject;
            if( "savetime".Equals( goParent.name ) )
            {
               RectTransform component = __instance.gameObject.GetComponent<RectTransform>();
               if( (double)component.sizeDelta.x >= 167 && (double)component.sizeDelta.x < 198 )
               {
                  component.sizeDelta = new Vector2( 198, component.sizeDelta.y );
                  __instance.horizontalAlignment = HorizontalAlignmentOptions.Left;
               }
            }
         }
         else if( "desc".Equals( __instance.name ) && __instance.gameObject.transform.parent != null )
         {
            var goParent = __instance.gameObject.transform.parent.gameObject;
            if( "detailpart".Equals( goParent.name ) )
            {
               RectTransform component = __instance.gameObject.GetComponent<RectTransform>();
               Vector2 sizeDelta = component.sizeDelta;
               if( (double)component.sizeDelta.x >= 527 && (double)component.sizeDelta.x < 557 )
               {
                  component.sizeDelta = new Vector2( 557, component.sizeDelta.y );
                  __instance.horizontalAlignment = HorizontalAlignmentOptions.Left;
               }
            }
         }
         return true;
      }
   }
}
