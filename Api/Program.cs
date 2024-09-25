
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

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
