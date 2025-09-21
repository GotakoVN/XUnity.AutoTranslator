using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XUnity.AutoTranslator.Plugin.Core;
using XUnity.AutoTranslator.Plugin.Core.Constants;
using XUnity.AutoTranslator.Plugin.Core.Endpoints;
using XUnity.AutoTranslator.Plugin.Core.Endpoints.Http;
using XUnity.AutoTranslator.Plugin.Core.Utilities;
using XUnity.AutoTranslator.Plugin.Core.Web;
using XUnity.Common.Logging;
using static System.Collections.Specialized.BitVector32;

namespace VietPhraseDD
{
   public class VPTranslateEndpoint : HttpEndpoint
   {
      public string[] arr_vni = new string[ 5 ] {"́","̣","̀","̉","̃"};
      private static readonly string HttpsServicePointTemplateUrl = "https://vietphrase.info/api/translate";
      private static readonly string RequestFreeKeyUrl = "https://vietphrase.info/api/request-free-key";
      private static readonly string ValidateKeyUrl = "https://vietphrase.info/validate-key";
      private static readonly string CheckAuthUrl = "https://vietphrase.info/check-auth";
      private static readonly Random RandomNumbers = new Random();
      private static readonly string[] ContentTypes = new string[ 1 ]
      {
      "application/x-www-form-urlencoded"
      };
      private static readonly string ContentType = VPTranslateEndpoint.ContentTypes[ VPTranslateEndpoint.RandomNumbers.Next( VPTranslateEndpoint.ContentTypes.Length ) ];
      private string[] authTokens;
      private string authToken;
      private string licenseKey;
      private string clientId;
      private bool useSingleToken = true;
      private int currentTokenIndex;
      private bool _hasSetup = false;
      public VPTranslateEndpoint()
      {
         this.authTokens = new string[ 3 ]
         {
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJWUC1GUkVFLUFDQkFEMTBFIiwidHlwZSI6ImZyZWUiLCJjbGllbnRJRCI6ImUwNWI0NDI1LTU2NzYtNDA2ZS1iOGEwLTJlN2IxYmQxZjA3MSIsImlhdCI6MTc1NTI3OTk2NCwiZXhwIjoxNzU1ODg0NzY0fQ.ycJSHchittQZuWAgiSlqHgsi9HurgrZ88fBnTCgbBLs",
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJWUC1GUkVFLUNGMEZEMjk5IiwidHlwZSI6ImZyZWUiLCJjbGllbnRJRCI6IjlkODQwZmU2LWE0OTEtNGIzMi1hZDMyLWI0N2JkYzI3ZTE5MiIsImlhdCI6MTc1NTI4MjE4NywiZXhwIjoxNzU1ODg2OTg3fQ.uRxPX8jQ1YAsjdI0pjPuSZHsnCfB-2m1iFawk2tuCuY",
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJWUC1GUkVFLUIyQjIyRkUzIiwidHlwZSI6ImZyZWUiLCJjbGllbnRJRCI6ImI0NWIxMTM4LTY2MTItNDU1Yy1hNmJjLWU0OWNhZDUyOWM4NSIsImlhdCI6MTc1NTI4MjIxNCwiZXhwIjoxNzU1ODg3MDE0fQ.Nywh5NGLzjaL8F5Brip3FFNAjzNixTiGmIjcML_6tTM"
         };
         this.currentTokenIndex = 0;
      }
      public override string Id => "VPTranslateDD";

      public override string FriendlyName => "VietPhrase DD";

      public override void Initialize( IInitializationContext context )
      {
         context.DisableCertificateChecksFor( "vietphrase.info" );
         this.clientId = context.GetOrCreateSetting( "VPTranslateDD", "ClientId", "");
         this.licenseKey = context.GetOrCreateSetting( "VPTranslateDD", "LicenseKey", "" );
         this.authToken = context.GetOrCreateSetting( "VPTranslateDD", "AuthToken", "");
         this.useSingleToken = context.GetOrCreateSetting( "VPTranslateDD", "UseSingleToken", true );
         if ( !string.IsNullOrEmpty( this.authToken ) )
         {
            this.authTokens[ 0 ] = this.authToken;
         }
         if ( string.IsNullOrEmpty( this.clientId ) )
         {
            this.clientId = Guid.NewGuid().ToString();
            context.SetSetting( "VPTranslateDD", "ClientId", clientId );
         }
         CheckLicense( context );
         SaveConfig( context );
      }

      private void SaveConfig( IInitializationContext context )
      {
         XuaLogger.AutoTranslator.Info( "Saving VietPhraseDD settings..." );
         if ( !string.IsNullOrEmpty( this.clientId ) )
         {
            context.SetSetting( "VPTranslateDD", "ClientId", clientId );
         }
         if ( !string.IsNullOrEmpty( this.authToken ))
         {
            context.SetSetting( "VPTranslateDD", "AuthToken", authToken );
         }
         if ( !string.IsNullOrEmpty( this.licenseKey ) )
         {
            context.SetSetting( "VPTranslateDD", "LicenseKey", licenseKey );
         }
         XuaLogger.AutoTranslator.Info( "VietPhraseDD settings saved." );
      }

      public override void OnCreateRequest( IHttpRequestCreationContext context )
      {
         JSONObject jsonObject = new JSONObject();
         jsonObject[ "text" ] = (JSONNode)context.UntranslatedText;
         jsonObject[ "type" ] = (JSONNode)0;
         jsonObject[ "html" ] = (JSONNode)true;
         jsonObject[ "structured" ] = (JSONNode)false;
         jsonObject[ "normalize" ] = (JSONNode)true;
         jsonObject[ "applyLuatNhan" ] = (JSONNode)false;
         jsonObject[ "clearspace" ] = (JSONNode)true;
         jsonObject[ "format" ] = (JSONNode)true;
         string data = ( (object)jsonObject ).ToString();
         //string authToken = this.authTokens[ this.currentTokenIndex ];
         //this.currentTokenIndex = ( this.currentTokenIndex + 1 ) % this.authTokens.Length;
         //if (this.useSingleToken )
         //{
         //   authToken = this.authToken;
         //}
         string authenticationToken = this.authToken;
         if ( string.IsNullOrEmpty( authenticationToken ) )
         {
            XuaLogger.AutoTranslator.Warn( "No auth token found. Cannot create request for VPTranslateDD." );
            context.Complete( null );
            return;
         }
         XUnityWebRequest request = new XUnityWebRequest( "POST", VPTranslateEndpoint.HttpsServicePointTemplateUrl, data );
         request.Headers[ HttpRequestHeader.ContentType ] = "application/json";
         request.Headers[ HttpRequestHeader.Accept ] = "application/json";
         request.Headers[ "Authorization" ] = "Bearer " + authToken;
         context.Complete( request );
      }

      private void CheckLicense( IInitializationContext context )
      {
         if( string.IsNullOrEmpty( this.licenseKey ) ) GetFreeLicenseKey( context );
         if( string.IsNullOrEmpty( this.authToken ) ) GetAuthToken( context );
         CheckAuth( context );
      }

      private void CheckAuth( IInitializationContext context )
      {
         if (!string.IsNullOrEmpty( this.authToken ) && !string.IsNullOrEmpty(licenseKey))
         {
            HttpResponseMessage response = null;
            try
            {
               var client = new HttpClient();
               client.DefaultRequestHeaders.Add( "cookie", "auth_token=" + authToken );
               response = client.GetAsync( VPTranslateEndpoint.CheckAuthUrl ).Result;
            }
            catch( Exception e )
            {
               XuaLogger.AutoTranslator.Warn( e, "An error occurred while check auth license VPTranslateDD. Proceeding without..." );
            }
            
            if( response.StatusCode != HttpStatusCode.OK )
            {
               XuaLogger.AutoTranslator.Warn( "An error occurred while check auth license VPTranslateDD. Proceeding without..." );
            }
            try
            {
               JSONObject result = JSONObject.Parse( response.Content.ReadAsStringAsync().Result ).AsObject;
               XuaLogger.AutoTranslator.Info( result.ToString());
               this.authToken = (string)result[ "bearerToken" ];
               context.SetSetting( "VPTranslateDD", "AuthToken", this.authToken );
            }
            catch( Exception e )
            {
               XuaLogger.AutoTranslator.Warn( e, "An error occurred while parse result in check auth license check VPTranslateDD. Proceeding without..." );
            }
         }
      }

      private void GetAuthToken( IInitializationContext context )
      {
         if( !string.IsNullOrEmpty( this.licenseKey ) )
         {
            HttpResponseMessage response = null;
            try
            {
               var client = new HttpClient();
               JSONObject jsonObject = new JSONObject();
               jsonObject[ "clientID" ] = (JSONNode)clientId;
               jsonObject[ "licenseKey" ] = (JSONNode)licenseKey;
               string data = jsonObject.ToString();
               StringContent jsonContent = new StringContent( data, Encoding.UTF8, "application/json" );
               response = client.PostAsync( VPTranslateEndpoint.ValidateKeyUrl, jsonContent ).Result;
            }
            catch( Exception e )
            {
               XuaLogger.AutoTranslator.Warn( e, "An error occurred while get license VPTranslateDD. Proceeding without..." );
            }
            
            if( response.StatusCode != HttpStatusCode.OK )
            {
               XuaLogger.AutoTranslator.Warn( "An error occurred while get license VPTranslateDD. Proceeding without..." );
               return;
            }
            try
            {
               JSONObject result = JSONObject.Parse( response.Content.ReadAsStringAsync().Result ).AsObject;
               foreach(string part in response.Headers.GetValues( "Set-Cookie" ))
               {
                  XuaLogger.AutoTranslator.Info( "cookie: " + part );
                  if( part.StartsWith( "auth_token=" ) )
                  {
                     this.authToken = part.Substring( "auth_token=".Length ).Trim();
                     break;
                  }
               }
                              
               if ( !string.IsNullOrEmpty( authToken ) )
               {
                  XuaLogger.AutoTranslator.Info( "key:" + licenseKey + " authToken: " + authToken );
               }
               else
               {
                  XuaLogger.AutoTranslator.Info( result.ToString() );
                  XuaLogger.AutoTranslator.Warn( "No auth_token cookie found in response. Proceeding without..." );
               }
            }
            catch( Exception e )
            {
               XuaLogger.AutoTranslator.Warn( e, "An error occurred while parse result in license check VPTranslateDD. Proceeding without..." );
            }
         }
      }

      private void GetFreeLicenseKey( IInitializationContext context )
      {
         HttpResponseMessage response = null;
         try
         {
            var client = new HttpClient();
            JSONObject jsonObject = new JSONObject();
            jsonObject[ "clientID" ] = (JSONNode)clientId;
            string data = jsonObject.ToString();
            StringContent jsonContent = new StringContent(data, Encoding.UTF8, "application/json" );
            response = client.PostAsync( VPTranslateEndpoint.RequestFreeKeyUrl, jsonContent ).Result;
         }
         catch( Exception e )
         {
            XuaLogger.AutoTranslator.Warn( e, "An error occurred while get license VPTranslateDD. Proceeding without..." );
         }
         
         if (response.StatusCode != HttpStatusCode.OK )
         {
            XuaLogger.AutoTranslator.Warn( "An error occurred while get license VPTranslateDD. Proceeding without..." );
            return;
         }
         try
         {
            JSONObject result = JSONObject.Parse( response.Content.ReadAsStringAsync().Result ).AsObject;
            licenseKey = (string) result[ "key" ];
            XuaLogger.AutoTranslator.Info( "Got key:" + licenseKey + " type: " + result[ "type" ] );
         }
         catch(Exception e )
         {
            XuaLogger.AutoTranslator.Warn( e, "An error occurred while parse result in license check VPTranslateDD. Proceeding without..." );
         }
      }

      public override void OnExtractTranslation( IHttpTranslationExtractionContext context )
      {
         string translatedText = this.FixVNIVietPhrase( (string)JSON.Parse( context.Response.Data.Trim().Replace( "\n", "" ).Replace( "\t", "" ) ).AsObject[ "result" ] );
         context.Complete( translatedText );
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
            string.IsNullOrEmpty( AutoTranslatorSettings.UserAgent ) ?
            UserAgents.Chrome_Win10_Latest : AutoTranslatorSettings.UserAgent;
         if( !( VPTranslateEndpoint.ContentType != null & isTranslationRequest ) )
            return;
         request.Headers[ HttpRequestHeader.ContentType ] = VPTranslateEndpoint.ContentType;
      }
   }
}
