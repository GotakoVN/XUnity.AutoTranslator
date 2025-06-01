using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XUnity.AutoTranslator.Plugin.Core.Constants;
using XUnity.AutoTranslator.Plugin.Core.Endpoints;
using XUnity.AutoTranslator.Plugin.Core.Endpoints.Http;
using XUnity.AutoTranslator.Plugin.Core.Web;
using XUnity.Common.Logging;

namespace ChiviTranslator
{
   public class ChiviTranslatorEndpoint : HttpEndpoint
   {
      private static readonly string httpsEndpoint = "https://chivi.app/_sp/qtran/c_gpt?pd=combine&rg=0&hs=1&ls=0&op=mtl";

      public override string Id => "ChiviTranslator";

      public override string FriendlyName => "Chivi Translator";

      private string CheckLanguage( string lang ) => lang == "zh-CN" || lang == "zh-Hans" ? "zh" : lang;

      private Dictionary<string, string> _dics = new Dictionary<string, string>() {
        {
          "ẻ",
          "ẻ"
        },
        {
          "é",
          "é"
        },
        {
          "è",
          "è"
        },
        {
          "ẹ",
          "ẹ"
        },
        {
          "ẽ",
          "ẽ"
        },
        {
          "ể",
          "ể"
        },
        {
          "ế",
          "ế"
        },
        {
          "ề",
          "ề"
        },
        {
          "ệ",
          "ệ"
        },
        {
          "ễ",
          "ễ"
        },
        {
          "ỷ",
          "ỷ"
        },
        {
          "ý",
          "ý"
        },
        {
          "ỳ",
          "ỳ"
        },
        {
          "ỵ",
          "ỵ"
        },
        {
          "ỹ",
          "ỹ"
        },
        {
          "ủ",
          "ủ"
        },
        {
          "ú",
          "ú"
        },
        {
          "ù",
          "ù"
        },
        {
          "ụ",
          "ụ"
        },
        {
          "ũ",
          "ũ"
        },
        {
          "ử",
          "ử"
        },
        {
          "ứ",
          "ứ"
        },
        {
          "ừ",
          "ừ"
        },
        {
          "ự",
          "ự"
        },
        {
          "ữ",
          "ữ"
        },
        {
          "ỉ",
          "ỉ"
        },
        {
          "í",
          "í"
        },
        {
          "ì",
          "ì"
        },
        {
          "ị",
          "ị"
        },
        {
          "ĩ",
          "ĩ"
        },
        {
          "ỏ",
          "ỏ"
        },
        {
          "ó",
          "ó"
        },
        {
          "ò",
          "ò"
        },
        {
          "ọ",
          "ọ"
        },
        {
          "õ",
          "õ"
        },
        {
          "ở",
          "ở"
        },
        {
          "ớ",
          "ớ"
        },
        {
          "ờ",
          "ờ"
        },
        {
          "ợ",
          "ợ"
        },
        {
          "ỡ",
          "ỡ"
        },
        {
          "ổ",
          "ổ"
        },
        {
          "ố",
          "ố"
        },
        {
          "ồ",
          "ồ"
        },
        {
          "ộ",
          "ộ"
        },
        {
          "ỗ",
          "ỗ"
        },
        {
          "ả",
          "ả"
        },
        {
          "á",
          "á"
        },
        {
          "à",
          "à"
        },
        {
          "ạ",
          "ạ"
        },
        {
          "ã",
          "ã"
        },
        {
          "ẳ",
          "ẳ"
        },
        {
          "ắ",
          "ắ"
        },
        {
          "ằ",
          "ằ"
        },
        {
          "ặ",
          "ặ"
        },
        {
          "ẵ",
          "ẵ"
        },
        {
          "ẩ",
          "ẩ"
        },
        {
          "ấ",
          "ấ"
        },
        {
          "ầ",
          "ầ"
        },
        {
          "ậ",
          "ậ"
        },
        {
          "ẫ",
          "ẫ"
        },
        {
          "Ẻ",
          "Ẻ"
        },
        {
          "É",
          "É"
        },
        {
          "È",
          "È"
        },
        {
          "Ẹ",
          "Ẹ"
        },
        {
          "Ẽ",
          "Ẽ"
        },
        {
          "Ể",
          "Ể"
        },
        {
          "Ế",
          "Ế"
        },
        {
          "Ề",
          "Ề"
        },
        {
          "Ệ",
          "Ệ"
        },
        {
          "Ễ",
          "Ễ"
        },
        {
          "Ỷ",
          "Ỷ"
        },
        {
          "Ý",
          "Ý"
        },
        {
          "Ỳ",
          "Ỳ"
        },
        {
          "Ỵ",
          "Ỵ"
        },
        {
          "Ỹ",
          "Ỹ"
        },
        {
          "Ủ",
          "Ủ"
        },
        {
          "Ú",
          "Ú"
        },
        {
          "Ù",
          "Ù"
        },
        {
          "Ụ",
          "Ụ"
        },
        {
          "Ũ",
          "Ũ"
        },
        {
          "Ử",
          "Ử"
        },
        {
          "Ứ",
          "Ứ"
        },
        {
          "Ừ",
          "Ừ"
        },
        {
          "Ự",
          "Ự"
        },
        {
          "Ữ",
          "Ữ"
        },
        {
          "Ỉ",
          "Ỉ"
        },
        {
          "Í",
          "Í"
        },
        {
          "Ì",
          "Ì"
        },
        {
          "Ị",
          "Ị"
        },
        {
          "Ĩ",
          "Ĩ"
        },
        {
          "Ỏ",
          "Ỏ"
        },
        {
          "Ó",
          "Ó"
        },
        {
          "Ò",
          "Ò"
        },
        {
          "Ọ",
          "Ọ"
        },
        {
          "Õ",
          "Õ"
        },
        {
          "Ở",
          "Ở"
        },
        {
          "Ớ",
          "Ớ"
        },
        {
          "Ờ",
          "Ờ"
        },
        {
          "Ợ",
          "Ợ"
        },
        {
          "Ỡ",
          "Ỡ"
        },
        {
          "Ổ",
          "Ổ"
        },
        {
          "Ố",
          "Ố"
        },
        {
          "Ồ",
          "Ồ"
        },
        {
          "Ộ",
          "Ộ"
        },
        {
          "Ỗ",
          "Ỗ"
        },
        {
          "Ả",
          "Ả"
        },
        {
          "Á",
          "Á"
        },
        {
          "À",
          "À"
        },
        {
          "Ạ",
          "Ạ"
        },
        {
          "Ã",
          "Ã"
        },
        {
          "Ẳ",
          "Ẳ"
        },
        {
          "Ắ",
          "Ắ"
        },
        {
          "Ằ",
          "Ằ"
        },
        {
          "Ặ",
          "Ặ"
        },
        {
          "Ẵ",
          "Ẵ"
        },
        {
          "Ẩ",
          "Ẩ"
        },
        {
          "Ấ",
          "Ấ"
        },
        {
          "Ầ",
          "Ầ"
        },
        {
          "Ậ",
          "Ậ"
        },
        {
          "Ẫ",
          "Ẫ"
        }
      };

      public override void Initialize( IInitializationContext context )
      {
         if( this.CheckLanguage( context.SourceLanguage ) != "zh" )
            throw new EndpointInitializationException( "The source language '" + context.SourceLanguage + "' is not supported." );
         if( this.CheckLanguage( context.DestinationLanguage ) != "vi" )
            throw new EndpointInitializationException( "The destination language '" + context.DestinationLanguage + "' is not supported." );
      }

      public override void OnCreateRequest( IHttpRequestCreationContext context )
      {
         string untranslatedText = context.UntranslatedText;
         string str = untranslatedText;
         XUnityWebRequest xunityWebRequest = new XUnityWebRequest( "POST", httpsEndpoint, str );
         xunityWebRequest.Headers[ HttpRequestHeader.UserAgent ] = UserAgents.Chrome_Win10_Latest;
         xunityWebRequest.Headers[ HttpRequestHeader.Accept ] = "*/*";
         xunityWebRequest.Headers[ HttpRequestHeader.AcceptLanguage ] = "en-US,en;q=0.9,vi;q=0.8";
         xunityWebRequest.Headers[ HttpRequestHeader.ContentType ] = "text/plain;charset=UTF-8";
         xunityWebRequest.Headers[ HttpRequestHeader.Referer ] = "https://chivi.app/mt/qtran";
         xunityWebRequest.Headers[ "referrerPolicy" ] = "strict-origin-when-cross-origin";
         xunityWebRequest.Headers[ "cache-control" ] = "no-cache";
         context.Complete( xunityWebRequest );
      }

      public override void OnExtractTranslation( IHttpTranslationExtractionContext context )
      {
         string data = context.Response.Data;
         if( string.IsNullOrEmpty( data ) )
         {
            context.Fail( "Received no translation." );
         }
         else
         {
            XuaLogger.Common.Info( $"Chivi translated: {data}" );
            string convertString = ConvertUnicodeToUTF8( data.Trim() );
            context.Complete( convertString );
         }
      }

      public string ConvertUnicodeToUTF8( string input )
      {
         foreach( KeyValuePair<string, string> keyValuePair in _dics)
            input = input.Replace( keyValuePair.Key, keyValuePair.Value );
         return input;
      }
   }
}
