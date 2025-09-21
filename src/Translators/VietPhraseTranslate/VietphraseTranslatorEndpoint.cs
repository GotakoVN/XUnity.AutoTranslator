using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnity.AutoTranslator.Plugin.Core.Endpoints;
using XUnity.Common.Logging;

namespace VietphraseTranslate
{
   internal class VietphraseTranslatorEndpoint : ITranslateEndpoint
   {
      public string[] arr_vni = new string[ 5 ]
      {
      "́",
      "̣",
      "̀",
      "̉",
      "̃"
      };
      private static readonly string HttpsServicePointTemplateUrl = "https://vietphrase.info/api/translate";
      private static readonly string HttpsServicePointOldTemplateUrl = "https://vietphrase.info/api/TranslateVietPhraseS";
      private static readonly string ReportUsageUrl = "https://vietphrase.info/record-usage";
      private static readonly string FeedbackUrl = "https://vietphrase.info/api/my-feedback";
      private static readonly string CheckAuthUrl = "https://vietphrase.info/check-auth";
      private static readonly string separator = "---------------------------";
      private static readonly Random RandomNumbers = new Random();
      private string currentAuthToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJWUC1GUkVFLUFDQkFEMTBFIiwidHlwZSI6ImZyZWUiLCJjbGllbnRJRCI6ImUwNWI0NDI1LTU2NzYtNDA2ZS1iOGEwLTJlN2IxYmQxZjA3MSIsImlhdCI6MTc1NTI3OTk2NCwiZXhwIjoxNzU1ODg0NzY0fQ.ycJSHchittQZuWAgiSlqHgsi9HurgrZ88fBnTCgbBLs";

      public string Id => "VietphraseTranslateV2";

      public string FriendlyName => "VietphraseTranslate V2";

      public int MaxConcurrency => 1;

      public int MaxTranslationsPerRequest { get; set; } = 5;
      public float RateLimitPerSecond { get; set; } = 1f;
      public bool UseOldServer { get; set; } = false;
      public static long CURRENT_TIMESTAMP  = DateTime.Now.Ticks;

      public void Initialize( IInitializationContext context )
      {
         context.DisableCertificateChecksFor( "vietphrase.info" );
         string authToken1 = context.GetOrCreateSetting( "VietPhraseTranslateV2", "AuthToken", currentAuthToken );
         MaxTranslationsPerRequest = context.GetOrCreateSetting( "VietPhraseTranslateV2", "MaxLinesPerRequest", 5 );
         RateLimitPerSecond = context.GetOrCreateSetting( "VietPhraseTranslateV2", "RateLimitPerSecond", 1f );
         UseOldServer = context.GetOrCreateSetting( "VietPhraseTranslateV2", "UseOldServer", false );
         if( !string.IsNullOrEmpty( authToken1 ) )
         {
            currentAuthToken = authToken1;
         }
      }

      public IEnumerator Translate( ITranslationContext context )
      {
         long timediff = DateTime.Now.Ticks - CURRENT_TIMESTAMP;
         long waitticks = (long)( TimeSpan.TicksPerSecond * RateLimitPerSecond );
         XuaLogger.AutoTranslator.Info( $"Wait: {RateLimitPerSecond}" );
         do
         {
            if( timediff >= waitticks )
            {
               CURRENT_TIMESTAMP = DateTime.Now.Ticks;
               break;
            }
            yield return null;
            timediff = DateTime.Now.Ticks - CURRENT_TIMESTAMP;
            // XuaLogger.AutoTranslator.Info( $"Wait: {timediff}" );
         } while( timediff < waitticks );
         Translator translateCall;
         if (UseOldServer)
         {
            translateCall = new TranslateVietphraseSCall( currentAuthToken, context.UntranslatedTexts );
         }
         else
         {
            translateCall = new TranslateWebCall( currentAuthToken, context.UntranslatedTexts );
         }
         translateCall.Run();
         XuaLogger.AutoTranslator.Info( $"Input: {context.UntranslatedTexts.Length} lines" );
         var iterator = translateCall.GetSupportedEnumerator();
         while( iterator.MoveNext() ) yield return iterator.Current;
         string[] result = translateCall.GetResult();
         ReportAndSyncWebCall reportAndSyncWebCall =
            new ReportAndSyncWebCall(
               currentAuthToken,
               string.Join( Translator.SPLIT_CHAR.ToString(), context.UntranslatedTexts ).Length);
         reportAndSyncWebCall.Run();
         iterator = reportAndSyncWebCall.GetSupportedEnumerator();
         while( iterator.MoveNext() ) yield return iterator.Current;
         context.Complete( result );
      }
   }
}
