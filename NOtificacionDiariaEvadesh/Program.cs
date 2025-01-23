using NOtificacionDiariaEvadesh.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;

class Program
{
    public static void Main(string[] args) => Run().GetAwaiter().GetResult();
    public static async Task Run()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/NotificacionesDiariasEvadesh.txt", rollingInterval: RollingInterval.Year)
            .WriteTo.Console(
             theme: SystemConsoleTheme.Literate)
            .CreateLogger();

        Log.Information("Inicia Proceso  Aplicación Consola NotificaciónDiariaEvadesh");

        var serviceCollection = new ServiceCollection();
        Configure(serviceCollection);

        var services = serviceCollection.BuildServiceProvider();

        var github = services.GetRequiredService<EvaPNotificacionesDiarias>();

        var response = await github.Principal();





        Console.ReadKey();
    }
    public static void Configure(IServiceCollection services)
    {
        IConfigurationRoot MyConfig = new ConfigurationBuilder().AddJsonFile("configuraciones.json").Build();
        //IConfigurationRoot root = MyConfig.Build();
        //var MyConfig = new ConfigurationBuilder().AddJsonFile("configuraciones.json").Build();
        var uri = MyConfig.GetValue<string>("api:url");
        services.AddHttpClient("api", c =>
        {
            c.BaseAddress = new Uri(uri);

            //c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json"); // GitHub API versioning
            //c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample"); // GitHub requires a user-agent
        })
        .AddTypedClient<EvaPNotificacionesDiarias>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

    }


}