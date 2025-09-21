using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XUnity.AutoTranslator.Plugin.Core;
using XUnity.AutoTranslator.Plugin.Core.Constants;
using XUnity.AutoTranslator.Plugin.Core.Endpoints;
using XUnity.AutoTranslator.Plugin.Core.Endpoints.Http;
using XUnity.AutoTranslator.Plugin.Core.Utilities;
using XUnity.AutoTranslator.Plugin.Core.Web;

namespace VietphraseTranslate
{
   public class VPTranslateEndpoint : HttpEndpoint
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
      private static readonly string[] ContentTypes = new string[ 1 ]
      {
      "application/x-www-form-urlencoded"
      };
      private static readonly string ContentType = VPTranslateEndpoint.ContentTypes[ VPTranslateEndpoint.RandomNumbers.Next( VPTranslateEndpoint.ContentTypes.Length ) ];
      private CookieContainer _cookieContainer;
      private string[] authTokens;
      private int currentTokenIndex;
      private string currentAuthToken;

      public VPTranslateEndpoint()
      {
         this._cookieContainer = new CookieContainer();
         this.authTokens = new string[ 3 ]
         {
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJWUC1GUkVFLUFDQkFEMTBFIiwidHlwZSI6ImZyZWUiLCJjbGllbnRJRCI6ImUwNWI0NDI1LTU2NzYtNDA2ZS1iOGEwLTJlN2IxYmQxZjA3MSIsImlhdCI6MTc1NTI3OTk2NCwiZXhwIjoxNzU1ODg0NzY0fQ.ycJSHchittQZuWAgiSlqHgsi9HurgrZ88fBnTCgbBLs",
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJWUC1GUkVFLUNGMEZEMjk5IiwidHlwZSI6ImZyZWUiLCJjbGllbnRJRCI6IjlkODQwZmU2LWE0OTEtNGIzMi1hZDMyLWI0N2JkYzI3ZTE5MiIsImlhdCI6MTc1NTI4MjE4NywiZXhwIjoxNzU1ODg2OTg3fQ.uRxPX8jQ1YAsjdI0pjPuSZHsnCfB-2m1iFawk2tuCuY",
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJWUC1GUkVFLUIyQjIyRkUzIiwidHlwZSI6ImZyZWUiLCJjbGllbnRJRCI6ImI0NWIxMTM4LTY2MTItNDU1Yy1hNmJjLWU0OWNhZDUyOWM4NSIsImlhdCI6MTc1NTI4MjIxNCwiZXhwIjoxNzU1ODg3MDE0fQ.Nywh5NGLzjaL8F5Brip3FFNAjzNixTiGmIjcML_6tTM"
         };
         this.currentTokenIndex = 0;
      }

      public override string Id => "VPBTranslate";

      public override string FriendlyName => "VietPhrase EDIT";

      public override void Initialize( IInitializationContext context )
      {
         context.DisableCertificateChecksFor( "vietphrase.info" );
         string authToken1 = context.GetOrCreateSetting( "VietPhraseTranslate", "AuthToken1", authTokens[ 0 ] );
         string authToken2 = context.GetOrCreateSetting( "VietPhraseTranslate", "AuthToken2", authTokens[ 1 ] );
         string authToken3 = context.GetOrCreateSetting( "VietPhraseTranslate", "AuthToken3", authTokens[ 2 ] );
         if (!string.IsNullOrEmpty( authToken1 )) 
         {
            this.authTokens[ 0 ] = authToken1;
         }
         if( !string.IsNullOrEmpty( authToken2 ) )
         {
            this.authTokens[ 1 ] = authToken2;
         }
         if( !string.IsNullOrEmpty( authToken3 ) )
         {
            this.authTokens[ 2 ] = authToken3;
         }
      }

      public override void OnCreateRequest( IHttpRequestCreationContext context )
      {
         JSONObject jsonObject = new JSONObject();
         // jsonObject[ "chineseContent" ] = (JSONNode)context.UntranslatedText;
         jsonObject[ "text" ] = (JSONNode)context.UntranslatedText;
         jsonObject[ "type" ] = (JSONNode)0;
         jsonObject[ "html" ] = (JSONNode)true;
         jsonObject[ "structured" ] = (JSONNode)false;
         jsonObject[ "normalize" ] = (JSONNode)true;
         jsonObject[ "applyLuatNhan" ] = (JSONNode)false;
         jsonObject[ "clearspace" ] = (JSONNode)true;
         jsonObject[ "format" ] = (JSONNode)true;
         string data = ( (object)jsonObject ).ToString();
         string authToken = this.authTokens[ this.currentTokenIndex ];
         this.currentTokenIndex = ( this.currentTokenIndex + 1 ) % this.authTokens.Length;
         currentAuthToken = authToken;

         XUnityWebRequest request = new XUnityWebRequest( "POST", VPTranslateEndpoint.HttpsServicePointTemplateUrl, data );
         request.Headers[ HttpRequestHeader.ContentType ] = "application/json";
         request.Headers[ HttpRequestHeader.Accept ] = "application/json";
         request.Headers[ "Authorization" ] = "Bearer " + authToken;
         request.Headers[ "cookie" ] = "auth_token=" + authToken;
         context.Complete( request );
      }

      public override void OnExtractTranslation( IHttpTranslationExtractionContext context )
      {
         string translatedText = this.FixVNIVietPhrase( (string)JSON.Parse( context.Response.Data.Trim().Replace( "\n", "" ).Replace( "\t", "" ) ).AsObject[ "result" ] );
         context.Complete( translatedText );

         // sync with VietPhrase server
         // XuaLogger.AutoTranslator.Info( $"Translation syncing" );
         try
         {
            ReportUsageFeedbackAndCheckLicense( context );
         }
         catch( Exception ex )
         {
            // XuaLogger.AutoTranslator.Error( $"Error during syncing: {ex}" );
         }
      }

      //public new IEnumerator Translate( ITranslationContext context )
      //{
      //   var httpContext = new VPHttpTranslationContext( context );

      //   // allow implementer of HttpEndpoint to do anything before starting translation
      //   var setup = OnBeforeTranslate( httpContext );
      //   if( setup != null )
      //   {
      //      while( setup.MoveNext() ) yield return setup.Current;
      //   }

      //   // prepare request
      //   OnCreateRequest( httpContext );
      //   if( httpContext.Request == null ) httpContext.Fail( "No request object was provided by the translator." );

      //   // execute request
      //   XuaLogger.AutoTranslator.Info( $"Execute translation" );
      //   var client = new XUnityWebClient();
      //   var response = client.Send( httpContext.Request );

      //   // wait for completion
      //   var iterator = response.GetSupportedEnumerator();
      //   while( iterator.MoveNext() ) yield return iterator.Current;

      //   if( response.IsTimedOut ) httpContext.Fail( "Error occurred while retrieving translation. Timeout." );

      //   httpContext.Response = response;
      //   OnInspectResponse( httpContext );

      //   // failure
      //   if( response.Error != null ) httpContext.Fail( "VP Error occurred while retrieving translation.", response.Error );

      //   // failure
      //   if( response.Data == null ) httpContext.Fail( "VP Error occurred while retrieving translation. Nothing was returned." );

      //   // XuaLogger.AutoTranslator.Info( $"CheckLicense: {clResult.Content.ReadAsStringAsync().Result}" );
      //   //JSONObject result = JSONObject.Parse( clResponse.Data ).AsObject;
      //   //authTokens[ 0 ] = (string)result[ "bearerToken" ];
      //   //authTokens[ 1 ] = (string)result[ "bearerToken" ];
      //   // extract text
      //   OnExtractTranslation( httpContext );
      //}

      private void ReportUsageFeedbackAndCheckLicense( IHttpTranslationExtractionContext context )
      {
         string etag = this.ReportUsage( context );
         this.Feedback( context, etag );
         this.CheckLicense( context, etag );
      }

      private void CheckLicense( IHttpTranslationExtractionContext context = null, string etag = null )
      {
         HttpClient httpClient = new HttpClient();
         httpClient.DefaultRequestHeaders.Add( "cookie", "auth_token=" + this.currentAuthToken );
         if( etag != null )
         {
            httpClient.DefaultRequestHeaders.Add( "If-None-Match", etag );
         }
         var task = httpClient.GetAsync( VPTranslateEndpoint.CheckAuthUrl );
         while( !task.IsCompleted ) Task.Delay( 5 ).Wait();
         var jsonString = task.Result.Content.ReadAsStringAsync().Result;
         JSONObject asObject = JSONNode.Parse( jsonString ).AsObject;
         this.authTokens[ 0 ] = (string)asObject[ "bearerToken" ];
         this.authTokens[ 1 ] = (string)asObject[ "bearerToken" ];
         this.authTokens[ 2 ] = (string)asObject[ "bearerToken" ];
         // XuaLogger.AutoTranslator.Info( $"CheckLicense: {jsonString}" );
      }

      private void Feedback( IHttpTranslationExtractionContext context, string etag )
      {
         HttpClient httpClient = new HttpClient();
         httpClient.DefaultRequestHeaders.Add( "cookie", "auth_token=" + this.currentAuthToken );
         if( etag != null )
         {
            httpClient.DefaultRequestHeaders.Add( "If-None-Match", etag );
         }
         httpClient.DefaultRequestHeaders.Add( "Authorization", "Bearer " + this.currentAuthToken );
         var task = httpClient.GetAsync( VPTranslateEndpoint.FeedbackUrl );
         while( !task.IsCompleted ) Task.Delay( 5 ).Wait();
      }

      private string ReportUsage( IHttpTranslationExtractionContext context )
      {
         HttpClient httpClient = new HttpClient();
         JSONObject jsonObject = new JSONObject();
         jsonObject[ "actionType" ] = (JSONNode)"text";
         jsonObject[ "count" ] = (JSONNode)context.UntranslatedText.Length;
         jsonObject[ "details" ] = (JSONNode)"Usage recorded from client.";
         StringContent content = new StringContent( ( (object)jsonObject ).ToString(), Encoding.UTF8, "application/json" );
         httpClient.DefaultRequestHeaders.Add( "cookie", "auth_token=" + this.currentAuthToken );
         Task<HttpResponseMessage> task1 = httpClient.PostAsync( VPTranslateEndpoint.ReportUsageUrl, (HttpContent)content );
         while( !task1.IsCompleted ) Task.Delay( 5 ).Wait();
         HttpResponseMessage result1 = task1.Result;
         string result2 = result1.Content.ReadAsStringAsync().Result;
         // XuaLogger.AutoTranslator.Info( $"ReportUsage: {result2}" );
         IEnumerable<string> values = result1.Headers.GetValues( "Etag" );
         return values == null ? (string)null : values.First<string>();
      }

      public string FixVNIVietPhrase( string text )
      {
         for( int index = 0; index < this.arr_vni.Length; ++index )
            text = text.Replace( this.arr_vni[ index ], "" );
         return text;
      }

      private void AddHeaders( XUnityWebRequest request, bool isTranslationRequest )
      {
         request.Headers[ HttpRequestHeader.UserAgent ] =
            string.IsNullOrEmpty( AutoTranslatorSettings.UserAgent ) ? UserAgents.Chrome_Win10_Latest : AutoTranslatorSettings.UserAgent;
         if( !( VPTranslateEndpoint.ContentType != null & isTranslationRequest ) )
            return;
         request.Headers[ HttpRequestHeader.ContentType ] = VPTranslateEndpoint.ContentType;
      }
   }
}
