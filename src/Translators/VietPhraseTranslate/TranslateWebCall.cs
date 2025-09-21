using SimpleJSON;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using XUnity.AutoTranslator.Plugin.Core.Shims;
using XUnity.Common.Logging;

namespace VietphraseTranslate
{
   internal class TranslateWebCall : Translator
   {
      public override string Url => "https://vietphrase.info/api/translate";

      // private string URL = "https://vietphrase.info/api/translate";

      public TranslateWebCall(string token, string[] untranslated)
      {
         this.token = token;
         this.untranslated = untranslated;
      }

      public override void Run()
      {
         string translateString = string.Join( SPLIT_CHAR.ToString(), untranslated );
         JSONObject jsonObject = new JSONObject();
         jsonObject[ "text" ] = (JSONNode)translateString;
         jsonObject[ "type" ] = (JSONNode)0;
         jsonObject[ "html" ] = (JSONNode)true;
         jsonObject[ "structured" ] = (JSONNode)false;
         jsonObject[ "normalize" ] = (JSONNode)true;
         jsonObject[ "applyLuatNhan" ] = (JSONNode)false;
         jsonObject[ "nguphap" ] = (JSONNode)false;
         jsonObject[ "clearspace" ] = (JSONNode)true;
         jsonObject[ "format" ] = (JSONNode)true;
         jsonObject[ "escapeHtml" ] = (JSONNode)false;
         StringContent content = new StringContent( jsonObject.ToString(), Encoding.UTF8, "application/json" );
         HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url);
         request.Method = HttpMethod.Post;
         request.Content = content;
         
         request.Headers.Add("Accept", "*/*");
         request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
         request.Headers.Add("Authorization", "Bearer " + token);
         request.Headers.Add("Cookie", "auth_token=" + this.token);
         HttpClient httpClient = new HttpClient();
         httpClient.DefaultRequestHeaders.Add( "Cookie", "auth_token=" + this.token );
         // XuaLogger.AutoTranslator.Info( $"Run translating call..." );
         httpClient.SendAsync(request).ContinueWith((task) =>
         {
            var response = task.Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
               var responseContent = response.Content.ReadAsStringAsync().Result;
               XuaLogger.AutoTranslator.Info( $"Result: {responseContent}" );
               string resultString =
               FixVNIVietPhrase(JSON.Parse( responseContent.Trim()
               .Replace( "\n", "" )
               .Replace( "\t", "" ) )
               .AsObject[ "result" ]);
               result.AddRange( resultString.Split( SPLIT_CHAR ) );
               for( int i=0;i <result.Count;i++ )
               {
                  if( result[ i ].Length > 1 )
                  {
                     result[ i ] = result[ i ].Substring( 0, 1 ).ToUpper() + result[ i ].Substring( 1 );
                  }
               }
               XuaLogger.AutoTranslator.Info( $"Translated: {string.Join(",", result.ToArray())}" );
            }
            else
            {
               XuaLogger.AutoTranslator.Error($"Error TranslateCall: HTTP {response.StatusCode} - {response.Content.ReadAsStringAsync().Result}");
            }
            isCompleted = true;
         });
      }
   }
}
