document.addEventListener("DOMContentLoaded", () => {
    if (document.getElementById("catalogueTableBody")) {
        
        loadCatalogue('/api/catalogue');

        
        const searchForm = document.getElementById("searchForm");
        if (searchForm) {
            searchForm.addEventListener("submit", (e) => {
                e.preventDefault(); 

                const team = document.getElementById("teamInput").value.trim();
                const onlyOffer = document.getElementById("checkOfertas").checked;

                
                const tagsSelected = Array.from(document.querySelectorAll('input[name="tags"]:checked')).map(cb => cb.value);

                
                const params = new URLSearchParams();
                if (team) params.append("team", team);
                if (onlyOffer) params.append("onlyOffer", "true");
                tagsSelected.forEach(tag => params.append("tags", tag));

                const queryString = params.toString();
                
                const url = queryString ? `/api/catalogue/search?${queryString}` : '/api/catalogue';

                loadCatalogue(url);
            });
        }

        
        const btnViewAll = document.getElementById("btnViewAll");
        if (btnViewAll) {
            btnViewAll.addEventListener("click", () => {
                if (searchForm) searchForm.reset(); 
                loadCatalogue('/api/catalogue');   
            });
        }
    }
});

function loadCatalogue(url) {
    const tableBody = document.getElementById("catalogueTableBody");
    const isLogged = getCookie("UserLogged") !== null && getCookie("UserLogged") !== "";

    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error("Error al cargar el catálogo.");
            return response.json();
        })
        .then(data => {
            tableBody.innerHTML = "";

            if (data.length === 0) {
                tableBody.innerHTML = `<tr><td colspan="9" class="text-center text-muted">No shirts found matching your criteria.</td></tr>`;
                return;
            }

            data.forEach(item => {
                const tr = document.createElement("tr");

                let originalPriceHtml = "";
                if (item.offerType !== "No" && item.offerType !== 0) {
                    originalPriceHtml = `<del>${item.tShirt.price.toFixed(2)} €</del>`;
                } else {
                    originalPriceHtml = `<span>${item.tShirt.price.toFixed(2)} €</span>`;
                }

                let addToCartBtnHtml = "";
                if (isLogged) {
                    addToCartBtnHtml = `
                    <form onsubmit="addToCart(event, ${item.tShirt.id})" class="m-0">
                        <button type="submit" class="btn btn-success btn-sm w-100 fw-bold">🛒 Add</button>
                    </form>`;
                }

                tr.innerHTML = `
                <td>${item.tShirt.id}</td>
                <td>${item.tShirt.team}</td>
                <td>${item.tShirt.size}</td>
                <td>${item.tShirt.isSecondHand ? "Yes" : "No"}</td>
                <td>${originalPriceHtml}</td>
                <td>${item.offerType}</td>
                <td>${item.offerDiscount !== null ? item.offerDiscount + "%" : "-"}</td>
                <td style="font-weight: bold; color: green;">
                    ${item.finalPrice.toFixed(2)} €
                </td>
                <td class="text-center" style="width: 120px;">
                    ${addToCartBtnHtml}
                </td>`;

                tableBody.appendChild(tr);
            });
        })
        .catch(error => {
            console.error("Error fetching catalogue:", error);
            tableBody.innerHTML = `<tr><td colspan="9" class="text-center text-danger fw-bold">Error loading catalogue data.</td></tr>`;
        });
}

function addToCart(event, shirtId) {
    event.preventDefault();

    fetch(`/api/user/addToCart/${shirtId}`, {
        method: 'POST'
    })
        .then(response => {
            if (response.ok) {
            } else if (response.status === 401) {
                window.location.href = "/user/UserLogin.html";
            } else {
                throw new Error("Error adding shirt to cart.");
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}