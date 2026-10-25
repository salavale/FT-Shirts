document.addEventListener("DOMContentLoaded", () => {
    const sellShirtForm = document.getElementById("sellShirtForm");

    if (sellShirtForm) {
        sellShirtForm.addEventListener("submit", (e) => {
            e.preventDefault();

            const team = document.getElementById("team").value.trim();
            const price = parseFloat(document.getElementById("price").value);
            const pName = document.getElementById("pName").value.trim();

            const pNumberValue = document.getElementById("pNumber").value;
            const pNumber = pNumberValue ? parseInt(pNumberValue) : null;
            const size = document.getElementById("size").value;

            const tagsSelected = Array.from(document.querySelectorAll('input[name="tags"]:checked')).map(cb => cb.value);

            const requestBody = {
                shirt: {
                    team: team,
                    price: price,
                    pName: pName,
                    pNumber: pNumber,
                    size: size
                },
                tags: tagsSelected
            };

            fetch('/api/catalogue', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            })
                .then(response => {
                    if (response.ok) {
                        window.location.href = "/catalogue/Index.html"; 
                    } else {
                        throw new Error("Error publishing the shirt.");
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                });
        });
    }
});