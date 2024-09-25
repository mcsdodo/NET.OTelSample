var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker.Worker>();

var host = builder.Build();
host.Run();