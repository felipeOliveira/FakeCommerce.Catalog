using System;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FakeCommerce.Catalog.Infrastructure.Data
{
    public class CatalogDataSource : IDisposable
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly ILogger<CatalogDataSource> _logger;
        private bool _disposed;

        public IDatabase Database { get; }

        public CatalogDataSource(string connectionString, ILogger<CatalogDataSource> logger)
        {
            _logger = logger;
            _connection = ConnectionMultiplexer.Connect(connectionString);
            _connection.ConnectionFailed += OnConnectionFailed;
            _connection.ConnectionRestored += OnConnectionRestored;

            Database = _connection.GetDatabase();
        }
        
        private void OnConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogInformation("Connection Restored... Endpoint: {ENDPOINT}\nFailure: {FAILURE}\nException: {EX}", e.EndPoint, e.FailureType, e.Exception);
        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogError(e.Exception, "Connection Broken. Endpoint: {ENDPOINT}\nFailure: {FAILURE}", e.EndPoint, e.FailureType);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.ConnectionFailed -= OnConnectionFailed;
                    _connection.ConnectionRestored -= OnConnectionRestored;
                    _connection.Dispose();
                    _disposed = true;
                }
            }
        }
    }
}
