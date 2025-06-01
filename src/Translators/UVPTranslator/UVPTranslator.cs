using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XUnity.AutoTranslator.Plugin.Core.Endpoints;
using XUnity.Common.Logging;

namespace UVPTranslator
{
   public class UVPTranslatorEndpoint : ITranslateEndpoint
   {
      public string Id => "UVPTranslator";

      public string FriendlyName => "UVPTranslator by 3D2T";

      public int MaxConcurrency => 3;

      public int MaxTranslationsPerRequest => 30;

      private bool initialized = false;

      public void Initialize( IInitializationContext context )
      {
         // TranslateCenter.AssemblyDirectory = context.TranslatorDirectory;
         TranslateCenter.Init( context );
         initialized = true;
      }

      public IEnumerator Translate( ITranslationContext context )
      {
         TranslateCall translateCall = new TranslateCall( context.UntranslatedTexts );
         translateCall.Run();
         XuaLogger.Common.Info( $"UVP input: {context.UntranslatedTexts.Length} lines" );
         var iterator = translateCall.GetSupportedEnumerator();
         while( iterator.MoveNext()) yield return iterator.Current;
         foreach(var line in translateCall.GetResult())
         {
            XuaLogger.Common.Info( $"UVP translated: {line}" );
         }
         context.Complete( translateCall.GetResult() );
         yield return null;
      }
   }
}
