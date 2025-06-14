document.getElementById("shorten-form").addEventListener("submit", async (e) => {
    e.preventDefault();

    const urlInput = document.getElementById("url-input");
    const expiryInput = document.getElementById("expiry-input");
    const result = document.getElementById("result");

    const originalUrl = urlInput.value.trim();
    const expiresInDays = parseInt(expiryInput.value, 10);

    // Basic validation
    if (!originalUrl) {
        result.textContent = "Please enter a valid URL.";
        result.classList.add("error");
        return;
    }

    if (
        isNaN(expiresInDays) ||
        expiresInDays < 1 ||
        expiresInDays > 365
    ) {
        result.textContent = "Expiry must be between 1 and 365 days.";
        result.classList.add("error");
        return;
    }

    try {
        const response = await fetch("/api/shorten", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ originalUrl, expiresInDays }),
        });

        if (!response.ok) throw new Error("Failed to shorten URL");

        const shortUrl = await response.text();

        result.textContent = `Short URL: `;
        result.classList.remove("error");

        const link = document.createElement("a");
        link.href = shortUrl;
        link.textContent = shortUrl;
        link.target = "_blank";
        link.rel = "noopener noreferrer";

        result.innerHTML = "";
        result.appendChild(document.createTextNode("Short URL: "));
        result.appendChild(link);
    } catch (error) {
        result.textContent = "Error shortening URL.";
        result.classList.add("error");
    }
});
