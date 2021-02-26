using Microsoft.Extensions.Logging;
using Polly;

namespace Orders
{
    public static class PollyExtension
    {
        private static readonly string LoggerKey = "LoggerKey";

        public static Context WithLogger(this Context context, ILogger logger)
        {
            context[LoggerKey] = logger;

            return context;
        }

        public static ILogger TryGetLogger(this Context context)
        {
            if (context.TryGetValue(LoggerKey, out object logger))
            {
                return logger as ILogger;
            }

            return null;
        }
    }
}