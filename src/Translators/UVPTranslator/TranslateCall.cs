using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using XUnity.AutoTranslator.Plugin.Core.Shims;

namespace UVPTranslator
{
    public class TranslateCall : CustomYieldInstructionShim
    {
        public bool isCompleted = false;
        private string[] untranslated;
        private List<string> result = new List<string>();
        public TranslateCall(string[] untranslated)
        {
            this.untranslated = untranslated;
        }
        public void Run()
        {
            Thread worker = new Thread(Worker_DoWork);
            worker.Start();
        }
        public string[] GetResult()
        {
            return result.ToArray();
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isCompleted = true;
        }

        private void Worker_DoWork()
        {
            foreach (string input in untranslated)
            {
                string translated = input;
                if (Utils.IsChinese(input))
                {
                    translated = TranslateCenter.Translate(input, "", " ", true);
                }
                result.Add(translated);
            }
            isCompleted = true;
        }

        public override bool keepWaiting => !isCompleted;
    }
}
