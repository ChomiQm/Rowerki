using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Project1.HealthChecks
{
    public class SqlServerHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly int _retryCount;
        private readonly int _delayOnFailure;
        public SqlServerHealthCheck(string connectionString, int retryCount, int delayOnFailure)
        {
            _connectionString = connectionString;
            _retryCount = retryCount;
            _delayOnFailure = delayOnFailure;
        }

        private async Task<bool> TryConnect()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            for (int connectionAttempt = 0; connectionAttempt <= _retryCount; connectionAttempt++)
            {
                if (await TryConnect())
                {
                    return HealthCheckResult.Healthy();
                }

                if (connectionAttempt != _retryCount)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(_delayOnFailure));
                }
            }
            return HealthCheckResult.Unhealthy();
        }

    }

}