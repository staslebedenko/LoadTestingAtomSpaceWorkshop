using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orders;
using Polly;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Orders
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(options =>
            {
                options.AddFilter("Orders", LogLevel.Information);
            });

            builder.Services.AddOptions<ProjectOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ProjectOptions").Bind(settings);
                });

            string sqlString = "Server=tcp:workshopstas.database.windows.net,1433;Initial Catalog=fun;Persist Security Info=False;User ID=fancyuser21;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string sqlPassword = "dsgruuire$wy453ujfd";
            string connectionString = new SqlConnectionStringBuilder(sqlString) { Password = sqlPassword }.ConnectionString;

            builder.Services.AddDbContextPool<PaperDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseSqlServer(connectionString, providerOptions => providerOptions.EnableRetryOnFailure());
                }
            });

            PaperDbContext.ExecuteMigrations(connectionString);

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton(options =>
            {
                return Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        2,
                        retryAttempt => TimeSpan.FromSeconds(1),
                        onRetry: (ex, retryCount, context) =>
                        {
                            ILogger log = context.TryGetLogger();
                            log?.LogError($"Polly retry {retryCount} for exception {ex}");
                        });
            });
        }
    }
}
