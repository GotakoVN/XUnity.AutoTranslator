using System;
using XUnity.Common.Logging.SimpleLogger.Logging.Formatters;

namespace XUnity.Common.Logging.SimpleLogger.Logging
{
    public class LogMessage
    {
        public DateTime DateTime { get; set; }
        public LogLevel Level { get; set; }
        public string Text { get; set; }
        public string CallingClass { get; set; }
        public string CallingMethod { get; set; }
        public int LineNumber { get; set; }

        public LogMessage() { }

        public LogMessage( LogLevel level, string text, DateTime dateTime, string callingClass, string callingMethod, int lineNumber)
        {
            Level = level;
            Text = text;
            DateTime = dateTime;
            CallingClass = callingClass;
            CallingMethod = callingMethod;
            LineNumber = lineNumber;
        }

        public override string ToString()
        {
            return new DefaultLoggerFormatter().ApplyFormat(this);
        }
    }
}
