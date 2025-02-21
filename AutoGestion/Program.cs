using AutoGestion;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using AutoGestion.Managers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);



builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var chatHttpClient = new HttpClient { BaseAddress = new Uri("https://radyesca.com/gestionclickapi") };
builder.Services.AddScoped(sp => chatHttpClient);

// Registro de ChatManager con HttpClient personalizado
var chatManager = new ChatManager(chatHttpClient);
builder.Services.AddScoped<IChatManager>(sp => chatManager);


//var url = builder.Configuration.GetValue<string>("Api:ApiUrl");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(url) });

builder.Services.AddSingleton<ApiServiceSelector>();

builder.Services.AddScoped(sp =>
{
    var apiUrlService = sp.GetRequiredService<ApiServiceSelector>();
    return new HttpClient { BaseAddress = new Uri(apiUrlService.GetApiUrl()) };
});

//builder.Services.AddTransient<IChatManager, ChatManager>();
//builder.Services.AddHttpClient("Autogestion.ServerAPI", client =>
//{
//    client.BaseAddress = new Uri("https://foscal.co/admautogestion/");
//    client.Timeout = TimeSpan.FromHours(12);
//});
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WhatsApi.Autogestion"));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices();
builder.Services.AddBootstrapBlazor();
await builder.Build().RunAsync();
