function CustomConfirm(title, message, type) {
    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            text: message,
            icon: type,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirmar'
        }).then((result) => {
            if (result.isConfirmed) {
                resolve(true);
            } else {
                resolve(false);
            }
        })
    });
}

window.ScrollToBottom = (elementName) => {
    element = document.getElementById(elementName);
    element.scrollTop = element.scrollHeight - element.clientHeight;
}

window.PlayAudio = (elementName) => {
    document.getElementById(elementName).play();
}

function descargarArchivo(nombreArchivo, bytesBase64) {
    const link = document.createElement('a');
    link.href = 'data:application/pdf;base64,' + bytesBase64;
    link.download = nombreArchivo;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}