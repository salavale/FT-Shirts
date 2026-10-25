document.addEventListener("DOMContentLoaded", () => {
    if (document.getElementById("cartTableBody")) {
        loadShoppingCart();
    }
});

function loadShoppingCart() {
    fetch('/api/user/shoppingCart')
        .then(response => {
            if (!response.ok) throw new Error("Error loading shopping cart.");
            return response.json();
        })
        .then(user => {
            const cartTableBody = document.getElementById("cartTableBody");
            const emptyCartMessage = document.getElementById("emptyCartMessage");
            const cartContent = document.getElementById("cartContent");
            const cartTotal = document.getElementById("cartTotal");

            const items = user.shoppingCart || [];

            if (items.length === 0) {
                if (emptyCartMessage) emptyCartMessage.style.display = "block";
                if (cartContent) cartContent.style.display = "none";
                return;
            }

            if (emptyCartMessage) emptyCartMessage.style.display = "none";
            if (cartContent) cartContent.style.display = "block";

            cartTableBody.innerHTML = "";
            let total = 0;

            items.forEach(item => {
                const tr = document.createElement("tr");

                let details = `Size: ${item.tShirt.size}`;
                if (item.tShirt.pName) {
                    details += ` | Player: ${item.tShirt.pName}`;
                    if (item.tShirt.pNumber) details += ` (${item.tShirt.pNumber})`;
                }

                tr.innerHTML = `
                    <td><strong>${item.tShirt.team}</strong></td>
                    <td>${details}</td>
                    <td class="text-end">${item.finalPrice.toFixed(2)} €</td>
                `;

                cartTableBody.appendChild(tr);
                total += item.finalPrice;
            });

            if (cartTotal) {
                cartTotal.textContent = total.toFixed(2);
            }
        })
        .catch(error => {
            console.error("Error fetching cart:", error);
        });
}