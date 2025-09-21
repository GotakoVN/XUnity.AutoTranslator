using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using XUnity.AutoTranslator.Plugin.Core.Shims;
using XUnity.Common.Logging;

namespace VietphraseTranslate
{
   internal class ReportAndSyncWebCall : CustomYieldInstructionShim
   {
      public bool isCompleted = false;
      public override bool keepWaiting => !isCompleted;
      private static readonly string ReportUsageUrl = "https://vietphrase.info/record-usage";
      private static readonly string FeedbackUrl = "https://vietphrase.info/api/my-feedback";
      private static readonly string CheckAuthUrl = "https://vietphrase.info/check-auth";
      private string token;
      private int count;

      public ReportAndSyncWebCall(string token, int count)
      {
         this.token = token;
         this.count = count;
      }
      public void Run()
      {
         var httpClient = new HttpClient();
         httpClient.DefaultRequestHeaders.Add("cookie", "auth_token=" + this.token);
         JSONObject jsonObject = new JSONObject();
         jsonObject[ "actionType" ] = (JSONNode)"text";
         jsonObject[ "count" ] = (JSONNode)count;
         jsonObject[ "details" ] = (JSONNode)"Usage recorded from client.";
         var message = new HttpRequestMessage(HttpMethod.Post, ReportUsageUrl);
         message.Method = HttpMethod.Post;
         message.Content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
         httpClient.SendAsync(message).ContinueWith((task) =>
         {
            var response = task.Result;
            
            IEnumerable<string> values = response.Headers.GetValues( "Etag" );
            string etagValue = values == null ? null : values.First();
            XuaLogger.AutoTranslator.Info( $"ReportUsage: HTTP {response.StatusCode} etag: {etagValue}" );
            if( response.StatusCode == HttpStatusCode.OK)
            {
               var feedBackMessage = new HttpRequestMessage( HttpMethod.Get, FeedbackUrl );
               feedBackMessage.Method = HttpMethod.Get;
               feedBackMessage.Headers.Add( "Authorization", "Bearer " + token );
               feedBackMessage.Headers.Add("cookie", "auth_token=" + this.token);
               if( etagValue != null )
               {
                  feedBackMessage.Headers.Add( "If-None-Match", etagValue );
               }
               httpClient.SendAsync( feedBackMessage ).ContinueWith( ( feedbackTask ) =>
               {
                  var checkAuthMessage = new HttpRequestMessage( HttpMethod.Get, CheckAuthUrl );
                  checkAuthMessage.Method = HttpMethod.Get;
                  checkAuthMessage.Headers.Add( "cookie", "auth_token=" + this.token );
                  if( etagValue != null )
                  {
                     feedBackMessage.Headers.Add( "If-None-Match", etagValue );
                  }
                  httpClient.SendAsync( checkAuthMessage ).ContinueWith( ( checkAuthTask ) =>
                  {
                     var checkAuthResponse = checkAuthTask.Result;
                     var jsonString = task.Result.Content.ReadAsStringAsync().Result;
                     XuaLogger.AutoTranslator.Info( $"CheckLicense: {jsonString}" );
                     isCompleted = true;
                  } );
                  // isCompleted = true;
               } );
            }
         } );
      }
   }
}
