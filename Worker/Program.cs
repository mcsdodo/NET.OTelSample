using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker.Worker>();

Action<OtlpExporterOptions> otlExporterOptions = opt =>
{
    opt.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!);
    opt.ExportProcessorType = ExportProcessorType.Batch;
    opt.Protocol = OtlpExportProtocol.Grpc;
};

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("otel-testing-worker"))
    .WithMetrics(metrics => metrics.AddHttpClientInstrumentation())
    .WithTracing(tracing => tracing.SetErrorStatusOnException().AddHttpClientInstrumentation()
        .AddSource(Worker.Worker.Source.Name));

builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
{
    metrics.AddOtlpExporter(otlExporterOptions);
}).ConfigureOpenTelemetryTracerProvider(traces =>
{
    traces.AddOtlpExporter(otlExporterOptions);
});

var host = builder.Build();
host.Run();