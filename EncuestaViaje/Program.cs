using EncuestaViaje.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

class Program
{
    public static void Main(string[] args) => Run().GetAwaiter().GetResult();
    public static async Task Run()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/EncuestaViaje.txt", rollingInterval: RollingInterval.Year)
            .CreateLogger();

        Log.Information("Ejecucion");

        var serviceCollection = new ServiceCollection();
        Configure(serviceCollection);

        var services = serviceCollection.BuildServiceProvider();

        var github = services.GetRequiredService<EnvioEncuestaViaje>();

        await github.EnviarEncuestaViaje();

        Console.ReadKey();
    }
    public static void Configure(IServiceCollection services)
    {
        var MyConfig = new ConfigurationBuilder().AddJsonFile("configuraciones.json").Build();
        var uri = MyConfig.GetValue<string>("api:url");

        services.AddHttpClient("api", c =>
        {
            c.BaseAddress = new Uri(uri);
            c.Timeout = TimeSpan.FromMinutes(30);

            //c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json"); // GitHub API versioning
            //c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample"); // GitHub requires a user-agent
        })
        .AddTypedClient<EnvioEncuestaViaje>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

    }

}