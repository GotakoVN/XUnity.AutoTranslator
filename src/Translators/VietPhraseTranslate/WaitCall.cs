using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnity.AutoTranslator.Plugin.Core.Shims;

namespace VietphraseTranslate
{
   internal class WaitCall : CustomYieldInstructionShim
   {
      public bool Waiting { set; get; } = true;
      public override bool keepWaiting => Waiting;
   }
}
