using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnity.AutoTranslator.Plugin.Core.Shims;

namespace VietphraseTranslate
{
   internal abstract class Translator : CustomYieldInstructionShim
   {
      public bool isCompleted = false;
      public override bool keepWaiting => !isCompleted;
      protected string[] untranslated;
      protected List<string> result = new List<string>();
      public static char SPLIT_CHAR = '¤';
      protected string token;
      public string[] arr_vni = new string[ 5 ]
      {
      "́",
      "̣",
      "̀",
      "̉",
      "̃"
      };
      public abstract string Url { get; }
      public abstract void Run();
      
      public string FixVNIVietPhrase( string text )
      {
         for( int index = 0; index < this.arr_vni.Length; ++index )
            text = text.Replace( this.arr_vni[ index ], "" );
         return text;
      }

      public string[] GetResult()
      {
         return result.ToArray();
      }
   }
}
