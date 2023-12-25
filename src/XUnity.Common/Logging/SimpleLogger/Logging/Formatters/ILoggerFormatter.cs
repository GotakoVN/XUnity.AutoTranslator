namespace XUnity.Common.Logging.SimpleLogger.Logging.Formatters
{
    public interface ILoggerFormatter
    {
        string ApplyFormat(LogMessage logMessage);
    }
}
