
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Autogestion.Shared.DTO.Chat;
using Autogestion.Shared.DTO.Empleado;
using System.Reflection.Metadata;
using AutoGestion.Helpers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
//using System.Net.Http;
//using System.Net.Http.Json;




namespace AutoGestion.Pages.Chat
{
    public partial class Chat
    {
        [CascadingParameter] public HubConnection hubConnection { get; set; }
        [Parameter] public string CurrentMessage { get; set; }
        [Parameter] public int CurrentUserId { get; set; }
        [Parameter] public string CurrentUserEmail { get; set; }
        private List<MessageDTO> messages = new List<MessageDTO>();
        bool _expanded = true;
        private EmpleadoReturnSapDTO datosempleado { get; set; }
        private bool _loading = false;

        public string LinkChat { get; set; } = "";
        private async Task SubmitAsync()
        {

        
            if (!string.IsNullOrEmpty(CurrentMessage) && ContactId > 0)
            {
              
                //Save Message to DB
                var chatHistory = new MessageDTO()
                {
                    Message = CurrentMessage,
                    ToEmpleadoId = ContactId,
                    CreatedDate = DateTime.Now,
                    FromEmpleadoId = datosempleado.Id.Value,

                };
                await _chatManager.SaveMessageAsync(chatHistory);
                chatHistory.FromEmpleadoId = CurrentUserId;
                await hubConnection.SendAsync("SendMessageAsync", chatHistory, datosempleado.ENAME);
                CurrentMessage = string.Empty;
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
            await JS.Sclroll("chatContainer");
        }
        protected override async Task OnInitializedAsync()
        {
    

            _loading = true;

            IConfigurationBuilder builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            // configurationSection.Key => FilePath
            // configurationSection.Value => C:\\temp\\logs\\output.txt
            IConfigurationSection configurationSection = configuration.GetSection("AppConfig").GetSection("Chathub");

            //Links Chat AnaA

            string LinkChat = configurationSection.Value;

            //LinkChat =  await Http.GetFromJsonAsync<string>(($"api/ConsultarConfiguracion/{"URLCHATGC"}"));

            datosempleado = await localStore.GetItemAsync<EmpleadoReturnSapDTO>("User");

           
            

            if (hubConnection == null)
            {
                ////hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7207/chathub").Build();
                //hubConnection = new HubConnectionBuilder().WithUrl(LinkChat).Build();
                hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7207/chathub").Build();
            }
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
            hubConnection.On<MessageDTO, string>("ReceiveMessage", async (message, userName) =>
            {

               

                if ((ContactId == message.ToEmpleadoId && datosempleado.Id == message.FromEmpleadoId) || (ContactId == message.FromEmpleadoId && datosempleado.Id == message.ToEmpleadoId))
                {
             
                    if ((ContactId == message.ToEmpleadoId && datosempleado.Id == message.FromEmpleadoId))
                    {
           
                        messages.Add(new MessageDTO { Message = message.Message, CreatedDate = message.CreatedDate, FromEmpleado = new EmpleadoDTO() { Nombres = CurrentUserEmail } });
                        await hubConnection.SendAsync("ChatNotificationAsync", $"Nuevo mennsaje de {userName}", ContactId, CurrentUserId);
                    }
                    else if ((ContactId == message.FromEmpleadoId && datosempleado.Id == message.ToEmpleadoId))
                    {
                        messages.Add(new MessageDTO { Message = message.Message, CreatedDate = message.CreatedDate, FromEmpleado = new EmpleadoDTO() { Nombres = ContactEmail } });
                    }
                    //await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
                  
                    await JS.Sclroll("chatContainer");
                    StateHasChanged();
                }
            });
        
            await GetUsersAsync();
            Users = await _chatManager.GetUsersAsync();
        
            var user = datosempleado.ENAME;
            CurrentUserId = datosempleado.Id.Value;
            CurrentUserEmail = datosempleado.ENAME;
            if ( ContactId > 0)
            {
                await LoadUserChat(ContactId);
            }
            _loading = false;
            StateHasChanged();
        }
        public List<EmpleadoDTO> ChatUsers = new List<EmpleadoDTO>();
        public List<EmpleadoDTO> Users = new List<EmpleadoDTO>();
        [Parameter] public string ContactEmail { get; set; }
        [Parameter] public int ContactId { get; set; }
        private bool dialogOpen = false;
        async Task LoadUserChat(int userId)
        {
            var contact = await _chatManager.GetUserDetailsAsync(userId);
            ContactId = contact.Id.Value;
            ContactEmail = contact.Nombres;
            _navigationManager.NavigateTo($"chat/{ContactId}");
            messages = new List<MessageDTO>();
            messages = await _chatManager.GetConversationAsync(ContactId,datosempleado.Id.Value);
        }
        private async Task GetUsersAsync()
        {
            ChatUsers = await _chatManager.GetUsersChatAsync(datosempleado.Id.Value);
        }

        private async Task HandleKeyPress(KeyboardEventArgs args)
        {

         
            if (args.Key == "Enter")
            {
                await SubmitAsync();
            }
        }

        private async void OnExpandCollapseClick()
        {
            
            _expanded = !_expanded;
        }

        private async void ShowUserSelectionDialog()
        {
           
         
            _expanded = !_expanded;
            dialogOpen = !dialogOpen;
            StateHasChanged();
        }
        private void SelectUser(EmpleadoDTO user)
        {
            if (!ChatUsers.Contains(user))
            {
                ChatUsers.Add(user);
            }


            _expanded = !_expanded;
            dialogOpen = !dialogOpen;
        }

    }
}
