using Microsoft.JSInterop;

namespace AutoGestion.Helpers
{
    public static class IJSExtensions
    {
        public static async Task<object> ShowMessage(this IJSRuntime js, string message)
        {
            return await js.InvokeAsync<object>("Swal.fire", message);
        }
        public static async Task<object> ShowMessage(this IJSRuntime js, string titulo ,string message, string tipo)
        {
            return await js.InvokeAsync<object>("Swal.fire", message,titulo,tipo);
        }
        public async static Task<bool> Confirm(this IJSRuntime js, string title, string message, typeMessageAlert typemessage)
        {
            return await js.InvokeAsync<bool>("CustomConfirm", title, message, typemessage.ToString());
        }

        public async static Task<string> Sclroll(this IJSRuntime js, string iddiv)
        {
            return await js.InvokeAsync<string>("ScrollToBottom", iddiv);
        }
        public async static Task<string> PlayAudio(this IJSRuntime js, string iddiv)
        {
            return await js.InvokeAsync<string>("PlayAudio", iddiv);
        }


        public enum typeMessageAlert
        {
            question, warning, error, succes, info
        }
    }
}
