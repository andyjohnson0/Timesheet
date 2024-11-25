// Download response from a url and save as a file
// Adapted from: https://stackoverflow.com/a/45905238/67316
// Licence: CC BY-SA 4.0

function downloadFile(url, fileName) {
    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error(`Failed to fetch file: ${response.statusText}`);
            }
            return response.blob();
        })
        .then(blob => {
            // Create an invisible link element
            const link = document.createElement("a");
            link.href = URL.createObjectURL(blob);
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            // Clean up
            URL.revokeObjectURL(link.href);
            document.body.removeChild(link);
        })
        .catch(error => console.error("Error downloading file:", error));
}
