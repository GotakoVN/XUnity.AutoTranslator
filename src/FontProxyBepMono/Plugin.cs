using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace Plugin
{
   [BepInPlugin( GUID: PluginData.Identifier, Name: PluginData.Name, Version: PluginData.Version )]
   public class Plugin : BaseUnityPlugin
    {
      public static ManualLogSource log;
      void Awake()
      {
         // Plugin startup logic
         log = Logger;
         FontStore.Init();
         Logger.LogMessage( "Plugin FontProxy is loading!" );
         SceneManager.sceneLoaded += SceneManager_sceneLoaded;
         var harmony = Harmony.CreateAndPatchAll(typeof(TextPatch));
         TMPSettingsPatch.Patch(harmony);
         Logger.LogMessage( "Plugin FontProxy is loaded OK!" );
      }

      private void SceneManager_sceneLoaded( Scene arg0, LoadSceneMode arg1 )
      {
         
      }
   }
}
