namespace Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var apiUrl = _configuration.GetValue<string>("API_URL")!;
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(apiUrl);


            var frequency = _configuration.GetValue<int>("WORKER_FREQUENCY_IN_SECONDS")!;

            _ = await client.PostAsync("/dosomething", content: null, stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(frequency / 2), stoppingToken);
            
            _ = await client.GetAsync("/fail", stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(frequency / 2), stoppingToken);
        }
    }
}