﻿using BepInEx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XUnity.ResourceRedirector.Constants;

namespace XUnity.ResourceRedirector.BepIn_5x
{
   [BepInPlugin( PluginData.Identifier, PluginData.Name, PluginData.Version )]
   public class ResourceRedirectorPlugin : BaseUnityPlugin
   {
      [Category( "Settings" )]
      [DisplayName( "Log all loaded resources" )]
      [Description( "Indicates whether or not all loaded resources should be logged to the console. Prepare for spam if enabled." )]
      public ConfigWrapper<bool> LogAllLoadedResources { get; set; }

      [Category( "Settings" )]
      [DisplayName( "Log callback order" )]
      [Description( "After registering a new callback, list all callbacks for a given event in their firing order. Prepare for spam if enabled." )]
      public ConfigWrapper<bool> LogCallbackOrder { get; set; }

      void Awake()
      {
         LogAllLoadedResources = new ConfigWrapper<bool>( "LogAllLoadedResources", this, false );
         ResourceRedirection.LogAllLoadedResources = LogAllLoadedResources.Value;
         LogAllLoadedResources.SettingChanged += ( s, e ) => ResourceRedirection.LogAllLoadedResources = LogAllLoadedResources.Value;

         LogCallbackOrder = new ConfigWrapper<bool>( "LogCallbackOrder", this, false );
         ResourceRedirection.LogCallbackOrder = LogCallbackOrder.Value;
         LogCallbackOrder.SettingChanged += ( s, e ) => ResourceRedirection.LogCallbackOrder = LogCallbackOrder.Value;

         Config.ConfigReloaded += Config_ConfigReloaded;
      }

      private void Config_ConfigReloaded()
      {
         ResourceRedirection.LogAllLoadedResources = LogAllLoadedResources.Value;
      }
   }
}
