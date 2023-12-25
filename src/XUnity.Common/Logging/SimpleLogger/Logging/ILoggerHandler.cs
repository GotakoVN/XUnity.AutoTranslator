namespace XUnity.Common.Logging.SimpleLogger.Logging
{
    public interface ILoggerHandler
    {
        void Publish(LogMessage logMessage);
    }
}
