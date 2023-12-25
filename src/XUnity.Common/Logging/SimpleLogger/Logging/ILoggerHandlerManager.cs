using System;

namespace XUnity.Common.Logging.SimpleLogger.Logging
{
    public interface ILoggerHandlerManager
    {
        ILoggerHandlerManager AddHandler(ILoggerHandler loggerHandler);
        ILoggerHandlerManager AddHandler(ILoggerHandler loggerHandler, Predicate<LogMessage> filter);

        bool RemoveHandler(ILoggerHandler loggerHandler);
    }
}
