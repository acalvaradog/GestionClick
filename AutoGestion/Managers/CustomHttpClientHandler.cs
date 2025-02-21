using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class CustomHttpClientHandler : DelegatingHandler
{
    private readonly ApiServiceSelector _apiServiceSelector;

    public CustomHttpClientHandler(ApiServiceSelector apiServiceSelector)
    {
        _apiServiceSelector = apiServiceSelector;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Cambia la URL de la API según la empresa seleccionada
        string baseUrl = _apiServiceSelector.GetApiUrl();
        request.RequestUri = new Uri(baseUrl + request.RequestUri.PathAndQuery);

        return await base.SendAsync(request, cancellationToken);
    }
}
