using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XUnity.AutoTranslator.Plugin.Core.Configuration
{
   public class SettingsHelper
   {
      public static void SetSuspendFontChanging( bool value )
      {
         Settings.SuspendFontChanging = value;
      }
   }
}
