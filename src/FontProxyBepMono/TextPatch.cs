using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Plugin
{
    public class TextPatch
    {
      [HarmonyPrefix]
      [HarmonyPatch( typeof( TextMeshProUGUI ), "OnEnable" )]
      public static bool TextMeshProUGUIPrefix( ref TextMeshProUGUI __instance )
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
