using Destructurama;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace PlatformX.Startup.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddSerilog(this IServiceCollection services, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();

            var logger = ConfigureLogger();

            loggingBuilder.AddSerilog(logger);

            services.AddLogging(lb =>
            {
                lb.AddSerilog(Log.Logger, true);
            });
        }

        public static void AddSerilog(this IServiceCollection services)
        {
            Log.Logger = ConfigureLogger();
            services.AddLogging(lb =>
            {
                lb.AddSerilog(Log.Logger, true);
            });
        }

        public static Logger ConfigureLogger()
        {
            var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT_NAME") ?? "EMPTY";
            var aspNetCoreEnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "EMPTY";
            var applicationName = Environment.GetEnvironmentVariable("APPLICATION_NAME") ?? "EMPTY";

            var microsoftLogLevel = environmentName == "prod" ? LogEventLevel.Error : LogEventLevel.Warning;
            var systemLogLevel = environmentName == "prod" ? LogEventLevel.Error : LogEventLevel.Warning;

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Default", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", microsoftLogLevel)
                .MinimumLevel.Override("System", systemLogLevel)
                .Destructure.UsingAttributes()
                .Enrich.WithProperty("EnvironmentName", environmentName)
                .Enrich.WithProperty("AspnetCoreEnvironmentName", aspNetCoreEnvironmentName)
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.FromLogContext()
                .WriteTo.Console(new JsonFormatter());

            var logger = loggerConfiguration.CreateLogger();

            return logger;
        }
    }
}
