using System;
using Microsoft.Extensions.Logging;

namespace FakeCommerce.Catalog.Infrastructure.Logging
{
    public class FakeCommerceLogger<T> : ILogger<T>
    {
        private readonly ILogger _logger;

        public FakeCommerceLogger(ILoggerFactory factory)
        {
            Type type = typeof(T);

            if (type.FullName.StartsWith("FakeCommerce"))
            {
                _logger = factory.CreateLogger(typeof(T).Name);
            }
            else
            {
                _logger = factory.CreateLogger(typeof(T).FullName);
            }


        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
