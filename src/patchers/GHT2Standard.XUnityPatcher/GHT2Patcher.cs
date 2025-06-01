using System;

namespace GHT2Standard.XUnityPatcher
{
   public static class GHT2Patcher
   {
      public static void Init()
      {
         HarmonyLib.Harmony harmony = new HarmonyLib.Harmony( "GHT2Patcher" );
         harmony.PatchAll( typeof( TextPatching ) );
         harmony.PatchAll( typeof( TMPPatching ) );
      }
   }
}
