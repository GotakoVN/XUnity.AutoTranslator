using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using HarmonyLib;

namespace Plugin
{
    public class TMPSettingsPatch
    {
        public static void Patch(HarmonyLib.Harmony harmony)
        {
            var original = typeof(Resources).GetMethod("Load", new[] { typeof(string), typeof(System.Type) });
            harmony.Patch(original,
                prefix: new HarmonyMethod(typeof(TMPSettingsPatch).GetMethod(nameof(Prefix))),
                postfix: new HarmonyMethod(typeof(TMPSettingsPatch).GetMethod(nameof(Postfix))));
        }

        public static bool Prefix(ref UnityEngine.Object __result, ref string __state, ref string path, System.Type systemTypeInstance)
        {
            __state = path;
            // if (path != null && path.StartsWith("UI/")) MelonLogger.Msg($"Override {__state} type={systemTypeInstance.ToString()}");
            return true;
        }
        public static void Postfix(ref UnityEngine.Object __result, ref string __state, System.Type systemTypeInstance)
        {
            if (__result != null && systemTypeInstance != null && systemTypeInstance.Name.Contains("TMP_Settings"))
            {
                TMP_Settings setting = (TMP_Settings)__result;
                if (setting != null)
                {
                    Plugin.log.LogMessage("Modify TMP Setting");
                    FontStore.TryAddMainSubAsset(setting);
                    FontStore.HasUpdatedSetting = true;
                }
            }
        }
    }
}
