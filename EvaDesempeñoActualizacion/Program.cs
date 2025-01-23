
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using EvaDesempeñoActualizacion.Services;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

class Program
{
    public static void Main(string[] args) => Run().GetAwaiter().GetResult();
    public static async Task Run()
    {
        try 
        {
        
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/ActualizacionEvaluacionDesempeño.txt", rollingInterval: RollingInterval.Year)
            //.WriteTo.Console(
            // theme: SystemConsoleTheme.Literate)
            .CreateLogger();

        Log.Information("Ejecucion Actualización Diaria Evadesh");

        var serviceCollection = new ServiceCollection();
        Configure(serviceCollection);

        var services = serviceCollection.BuildServiceProvider();

        var github = services.GetRequiredService<ActualizacionDiaria>();

        var Resultado = await github.ActualizacionDiariaM();

        }
        catch (Exception ex) 
        {
            Log.Information("Error de Ejecución: " +ex);
        }
        Console.ReadKey();
    }
    public static void Configure(IServiceCollection services)
    {
        var MyConfig = new ConfigurationBuilder().AddJsonFile("configuraciones.json").Build();
        var uri = MyConfig.GetValue<string>("api:url");
        services.AddHttpClient("api", c =>
        {
            c.BaseAddress = new Uri(uri);

            //c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json"); // GitHub API versioning
            //c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample"); // GitHub requires a user-agent
        })
        .AddTypedClient<ActualizacionDiaria>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

    }


}