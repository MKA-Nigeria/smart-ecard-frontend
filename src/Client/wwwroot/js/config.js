async function loadConfig() {
    try {
        const response = await fetch('/api/config');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const config = await response.json();
        document.getElementById('smart-e-card-logo').src = config.imageSrc;
    } catch (error) {
        console.error('Error fetching configuration:', error);
    }
}

window.addEventListener('load', loadConfig);
