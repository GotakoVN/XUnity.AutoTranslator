using System;
using System.IO;
using XUnity.Common.Logging.SimpleLogger.Logging.Formatters;

namespace XUnity.Common.Logging.SimpleLogger.Logging.Handlers
{
    public class FileLoggerHandler : ILoggerHandler
    {
        private readonly string _fileName;
        private readonly string _directory;
        private readonly ILoggerFormatter _loggerFormatter;
        private static bool _firstTime = true;

        public FileLoggerHandler() : this(CreateFileName()) { }

        public FileLoggerHandler(string fileName) : this(fileName, string.Empty) { }

        public FileLoggerHandler(string fileName, string directory) : this(new DefaultLoggerFormatter(), fileName, directory) { }

        public FileLoggerHandler(ILoggerFormatter loggerFormatter) : this(loggerFormatter, CreateFileName()) { }

        public FileLoggerHandler(ILoggerFormatter loggerFormatter, string fileName) : this(loggerFormatter, fileName, string.Empty) { }

        public FileLoggerHandler(ILoggerFormatter loggerFormatter, string fileName, string directory)
        {
            _loggerFormatter = loggerFormatter;
            _fileName = fileName;
            _directory = directory;
        }

        public void Publish(LogMessage logMessage)
        {
            if (!string.IsNullOrEmpty(_directory))
            {
                var directoryInfo = new DirectoryInfo(_directory);
                if (!directoryInfo.Exists)
                    directoryInfo.Create();
            }
            string fileName = Path.Combine( _directory, _fileName );
            if (File.Exists(fileName) && _firstTime)
            {
               File.Delete(fileName);
               _firstTime = false;
            }
            using (var writer = new StreamWriter(File.Open(Path.Combine(_directory, _fileName), FileMode.Append)))
                writer.WriteLine(_loggerFormatter.ApplyFormat(logMessage));
        }

        private static string CreateFileName()
        {
            var currentDate = DateTime.Now;
            var guid = Guid.NewGuid();
            return "Log_Reipatcher.log";
            //return string.Format("Log_{0:0000}{1:00}{2:00}-{3:00}{4:00}_{5}.log",
            //    currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, currentDate.Minute, guid);
        }
    }
}
