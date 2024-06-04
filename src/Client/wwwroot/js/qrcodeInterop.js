window.qrcodeInterop = {
    generateQRCode: function (elementId, text) {
        console.log("Generating QR Code for:", elementId, text);
        var element = document.getElementById(elementId);
        if (element) {
            element.innerHTML = ""; // Clear the element before generating the QR code
            new QRCode(element, {
                text: text,
                width: 150,
                height: 150,
            });
            console.log("QR Code generated.");
        } else {
            console.log("Element not found:", elementId);
        }
    },
    clearQRCode: function (elementId) {
        console.log("Clearing QR Code for:", elementId);
        var element = document.getElementById(elementId);
        if (element) {
            element.innerHTML = "";
        } else {
            console.log("Element not found:", elementId);
        }
    }
};
