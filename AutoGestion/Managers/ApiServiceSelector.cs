using Microsoft.Extensions.Configuration;

public class ApiServiceSelector
{
    private readonly IConfiguration _configuration;
    private string _empresaSeleccionada = "3000"; // Empresa por defecto

    public ApiServiceSelector(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SetEmpresa(string empresa)
    {
        _empresaSeleccionada = empresa;
    }

    public string GetApiUrl()
    {
        return _configuration[$"ApiUrls:ApiURL{_empresaSeleccionada}"];
    }
}