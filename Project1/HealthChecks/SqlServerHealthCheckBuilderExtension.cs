using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace Project1.HealthChecks
{
    public static class SqlServerHealthCheckBuilderExtension
    {
        private const string NAME = "pysiecdb";

        public static IHealthChecksBuilder AddSqlServerHealthCheck(this IHealthChecksBuilder builder,
            string connectionString,
            int retryCount,
            int delayOnFailure,
            string? name = null,
            HealthStatus? failureStatus = null,
            IEnumerable<string> tags = null)
        {
            IEnumerable<string> safeTags = tags ?? Enumerable.Empty<string>();
            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => new SqlServerHealthCheck(connectionString, retryCount, delayOnFailure),
                failureStatus,
                tags));

        }
    }
}