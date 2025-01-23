const serviceWorkerFileName = '/service-worker.js';
const swInstalledEvent = 'installed';
const staticCachePrefix = 'blazor-cache-v';
const updateAlertMessage = 'Update available. Reload the page when convenient.';
const blazorAssembly = 'AutoGestion';
const blazorInstallMethod = 'PWAInstallable';




var config = {
    messagingSenderId: "394647742681"
};
firebase.initializeApp(config);
const messaging = firebase.messaging();


window.updateAvailable = new Promise(function (resolve, reject) {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register(serviceWorkerFileName)
            .then(function (registration) {

                messaging.useServiceWorker(registration);

                // Request for permission
                messaging.requestPermission()
                    .then(function () {
                        console.log('Notification permission granted.');
                        // TODO(developer): Retrieve an Instance ID token for use with FCM.
                        messaging.getToken()
                            .then(function (currentToken) {
                                if (currentToken) {
                                    console.log('Token: ' + currentToken)
                                    sendTokenToServer(currentToken);
                                } else {
                                    console.log('No Instance ID token available. Request permission to generate one.');
                                    setTokenSentToServer(false);
                                }
                            })
                            .catch(function (err) {
                                console.log('An error occurred while retrieving token. ', err);
                                setTokenSentToServer(false);
                            });
                    })
                    .catch(function (err) {
                        console.log('Unable to get permission to notify.', err);
                    });

                console.log('Registration successful, scope is:', registration.scope);
                registration.onupdatefound = () => {
                    const installingWorker = registration.installing;
                    installingWorker.onstatechange = () => {
                        switch (installingWorker.state) {
                            case swInstalledEvent:
                                if (navigator.serviceWorker.controller) {
                                    resolve(true);
                                } else {
                                    resolve(false);
                                }
                                break;
                            default:
                        }
                    };
                };
            })
            .catch(error =>
                console.log('Service worker registration failed, error:', error));
    }
});
window['updateAvailable']
    .then(isAvailable => {
        if (isAvailable) {
            alert(updateAlertMessage);
        }
    });
function showAddToHomeScreen() {
    DotNet.invokeMethodAsync(blazorAssembly, blazorInstallMethod)
        .then(function () {  }, function (er) { setTimeout(showAddToHomeScreen, 1000); });
}

window.BlazorPWA = {
    installPWA: function () {
        if (window.PWADeferredPrompt) {
            window.PWADeferredPrompt.prompt();
            window.PWADeferredPrompt.userChoice
                .then(function (choiceResult) {
                    window.PWADeferredPrompt = null;
                });
        }
    }
};
window.addEventListener('beforeinstallprompt', function (e) {
    // Prevent Chrome 67 and earlier from automatically showing the prompt
    e.preventDefault();
    // Stash the event so it can be triggered later.
    window.PWADeferredPrompt = e;

    showAddToHomeScreen();

});



// Handle incoming messages
messaging.onMessage(function (payload) {
    console.log("Notification received: ", payload);


    Swal.fire({
        title: payload.notification.title,
        text: payload.notification.body,
        imageUrl: payload.notification.icon,
        showClass: {
            popup: 'animate__animated animate__fadeInDown'
        },
        hideClass: {
            popup: 'animate__animated animate__fadeOutUp'
        }
    })
});





// Callback fired if Instance ID token is updated.
messaging.onTokenRefresh(function () {
    messaging.getToken()
        .then(function (refreshedToken) {
            console.log('Token refreshed.');
            // Indicate that the new Instance ID token has not yet been sent 
            // to the app server.
            setTokenSentToServer(false);
            // Send Instance ID token to app server.
            sendTokenToServer(refreshedToken);
        })
        .catch(function (err) {
            console.log('Unable to retrieve refreshed token ', err);
        });
});

// Send the Instance ID token your application server, so that it can:
// - send messages back to this app
// - subscribe/unsubscribe the token from topics
function sendTokenToServer(currentToken) {
 /*   document.getElementById("txt_token").value = currentToken;*/
  //  EnviarToken(currentToken);
    if (!isTokenSentToServer()) {
        console.log('Sending token to server...');
        EnviarToken(currentToken);
        // TODO(developer): Send the current token to your server.
        setTokenSentToServer(true);
    } else {

        console.log('Token already sent to server so won\'t send it again ' +
            'unless it changes');
    }
}

function isTokenSentToServer() {
    return window.localStorage.getItem('sentToServer') == 1;
}

function setTokenSentToServer(sent) {

    window.localStorage.setItem('sentToServer', sent ? 1 : 0);
}

function EnviarToken(token) {

    console.log("enviando token");
    window.localStorage.setItem('token', token);
    //DotNet.invokeMethodAsync("AutoGestion", "ObtenerToken")
    //    .then(resultado => {
    //        console.log("recibido desde javascript" + resultado)
    //    });

    //var tokenbase = btoa(token);
    //var nroempleado = document.getElementById("txt_codigoempleado").value;
    //var documento = document.getElementById("login").value;
    //var url = servidor + "/api/dispositivos";
    //var myKeyVals = { token: 1, NroEmpleado: 5, Documento: 2 }

    //var DatosDispositivo = { "token": "", "NroEmpleado": "", "Documento": "" };
    //DatosDispositivo.token = tokenbase;
    //DatosDispositivo.NroEmpleado = nroempleado;
    //DatosDispositivo.Documento = documento;

    //var EnviarDatosDispositivos = JSON.stringify(DatosDispositivo);


    //$.ajax({
    //    url: url,
    //    type: 'POST',
    //    data: EnviarDatosDispositivos,
    //    contentType: 'application/json; charset=utf-8',
    //    dataType: 'json',
    //    success: function (response) {

    //    },
    //    error: function () {

    //    }
    //});



}
