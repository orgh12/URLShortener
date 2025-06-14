document.getElementById("shorten-form").addEventListener("submit", async (e) => {
    e.preventDefault();

    const urlInput = document.getElementById("url-input");
    const expiryInput = document.getElementById("expiry-input");
    const customInput = document.getElementById("custom-url-input");
    const result = document.getElementById("result");

    const originalUrl = urlInput.value.trim();
    const expiresInDays = parseFloat(expiryInput.value);
    const customUrl = customInput.value.trim();

    result.textContent = "";
    result.classList.remove("error");

    if (!originalUrl) {
        result.textContent = "Please enter a valid URL.";
        result.classList.add("error");
        return;
    }

    if (isNaN(expiresInDays) || expiresInDays <= 0) {
        result.textContent = "Expiry must be a number greater than 0.";
        result.classList.add("error");
        return;
    }

    try {
        let endpoint = "/api/shorten";
        if (customUrl) {
            endpoint += `?CustomUrl=${encodeURIComponent(customUrl)}`;
        }

        const response = await fetch(endpoint, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ originalUrl, expiresInDays }),
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            result.textContent = errorMessage || "Error shortening URL.";
            result.classList.add("error");
            return;
        }

        const shortUrl = await response.text();

        result.textContent = "";
        result.classList.remove("error");

        const link = document.createElement("a");
        link.href = shortUrl;
        link.textContent = shortUrl;
        link.target = "_blank";
        link.rel = "noopener noreferrer";

        result.appendChild(document.createTextNode("Short URL: "));
        result.appendChild(link);
    } catch (error) {
        result.textContent = "Network error. Please try again.";
        result.classList.add("error");
    }
});
