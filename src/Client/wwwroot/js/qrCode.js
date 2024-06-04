function generateQRCode(data) {
    var qrcode = new QRCode(document.getElementById("qrcode"), {
        text: data,
        width: 128,
        height: 128,
        colorDark: "#000000",
        colorLight: "#ffffff"
        
    });
    return qrcode.canvas.toDataURL("image/png");
}