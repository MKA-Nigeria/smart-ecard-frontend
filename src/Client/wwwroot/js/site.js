function processImage(base64String) {
    return new Promise((resolve, reject) => {
        var img = new Image();
        img.src = 'data:image/jpeg;base64,' + base64String;

        img.onload = function () {
            var canvas = document.createElement('canvas');
            var size = Math.max(img.width, img.height);
            canvas.width = size;
            canvas.height = size;
            var ctx = canvas.getContext('2d');

            // Fill with white background
            ctx.fillStyle = "white";
            ctx.fillRect(0, 0, size, size);

            // Draw the image in the center
            var offsetX = (size - img.width) / 2;
            var offsetY = (size - img.height) / 2;
            ctx.drawImage(img, offsetX, offsetY);

            // Get the processed image as a base64 string
            var processedBase64String = canvas.toDataURL('image/jpeg').split(',')[1];
            resolve(processedBase64String);
        };

        img.onerror = function (err) {
            reject(err);
        };
    });
}

console.log("site.js loaded");

window.processImage = function (base64String) {
    console.log("processImage called");
    // The rest of your code...
}

window.addEventListener('load', processImage);