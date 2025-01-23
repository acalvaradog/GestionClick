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