window.imageResizerInterop = {
    resizeImage: function (base64Image, width, height) {
        return new Promise((resolve, reject) => {
            const img = new Image();
            img.src = base64Image;
            img.onload = () => {
                const canvas = document.createElement('canvas');
                const ctx = canvas.getContext('2d');
                canvas.width = width;
                canvas.height = height;

                // Fill the canvas with white background
                ctx.fillStyle = 'white';
                ctx.fillRect(0, 0, width, height);

                // Draw the image onto the canvas
                const x = (width - img.width) / 2;
                const y = (height - img.height) / 2;
                ctx.drawImage(img, x, y);

                // Get the resized image as base64 string
                const resizedImage = canvas.toDataURL('image/jpeg'); // You can change format if needed
                resolve(resizedImage);
            };
            img.onerror = (error) => reject(error);
        });
    }
};
