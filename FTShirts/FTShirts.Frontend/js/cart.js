const API_URL = 'http://127.0.0.1:5000';

document.addEventListener("DOMContentLoaded", () => {
    if (document.getElementById("cartTableBody")) {
        loadShoppingCart();
    }
});

function loadShoppingCart() {
    fetch(`${API_URL}/api/cart`, {
        credentials: 'include'
    })
        .then(response => {
            if (!response.ok) throw new Error("Error loading shopping cart.");
            return response.json();
        })
        .then(user => {
            const cartTableBody = document.getElementById("cartTableBody");
            const emptyCartMessage = document.getElementById("emptyCartMessage");
            const cartContent = document.getElementById("cartContent");
            const cartTotal = document.getElementById("cartTotal");

            const items = (user && user.shoppingCart && Array.isArray(user.shoppingCart.items))
                ? user.shoppingCart.items
                : [];

            if (items.length === 0) {
                if (emptyCartMessage) emptyCartMessage.style.display = "block";
                if (cartContent) cartContent.style.display = "none";
                if (cartTotal) cartTotal.textContent = "0.00";
                return;
            }

            if (emptyCartMessage) emptyCartMessage.style.display = "none";
            if (cartContent) cartContent.style.display = "block";

            cartTableBody.innerHTML = "";
            let total = 0;

            items.forEach(item => {
                const tr = document.createElement("tr");
                const shirt = item.tShirt || {};
                const price = Number(item.finalPrice ?? 0);

                let details = `Size: ${shirt.size ?? "-"}`;
                if (shirt.pName) {
                    details += ` | Player: ${shirt.pName}`;
                    if (shirt.pNumber) details += ` (${shirt.pNumber})`;
                }

                tr.innerHTML = `
                    <td><strong>${shirt.team ?? "Unknown"}</strong></td>
                    <td>${details}</td>
                    <td class="text-end">${price.toFixed(2)} €</td>
                `;

                cartTableBody.appendChild(tr);
                total += price;
            });

            if (cartTotal) {
                cartTotal.textContent = total.toFixed(2);
            }
        })
        .catch(error => {
            console.error("Error fetching cart:", error);
        });
}