
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

Action<OtlpExporterOptions> otlExporterOptions = opt =>
{
    opt.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!);
    opt.ExportProcessorType = ExportProcessorType.Batch;
    opt.Protocol = OtlpExportProtocol.Grpc;
};

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("otel-testing-api"))
    .WithMetrics(metrics => metrics.AddHttpClientInstrumentation())
    .WithTracing(tracing => tracing.SetErrorStatusOnException().AddHttpClientInstrumentation());

builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
{
    metrics.AddAspNetCoreInstrumentation();
    metrics.AddOtlpExporter(otlExporterOptions);
}).ConfigureOpenTelemetryTracerProvider(traces =>
{
    traces.AddAspNetCoreInstrumentation();
    traces.AddOtlpExporter(otlExporterOptions);
});


var app = builder.Build();
var random = new Random();

app.MapPost("/dosomething", async () =>
{
    await Task.Delay(random.Next(100, 500));
    return Results.Accepted();
});

app.MapGet("/fail", async () =>
{
    await Task.Delay(random.Next(100, 500));
    throw new Exception("Something happened");
});

app.Run();
