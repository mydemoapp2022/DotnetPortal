using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace UI.EmployerPortal.Web.Startup;

internal static class OpenTelemetryExt
{
    public static WebApplicationBuilder AddOpenTelemetryInDevelopment(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            var services = builder.Services;
            services
                .AddOpenTelemetry()
                .ConfigureResource(x =>
                {
                    x.AddService("UI-EmployerPortal");
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddWcfInstrumentation()
                        .AddOtlpExporter();
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter();
                });

            var logging = builder.Logging;
            logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
                logging.ParseStateValues = true;
                logging.AddOtlpExporter();
            });

        }

        return builder;
    }
}
