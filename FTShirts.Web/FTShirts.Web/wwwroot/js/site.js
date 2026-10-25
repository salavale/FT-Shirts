document.addEventListener("DOMContentLoaded", () => {

    const headerPlaceholder = document.getElementById("header-placeholder");

    if (headerPlaceholder) {
        fetch('/header.html')
            .then(response => {
                if (!response.ok) throw new Error("No se pudo cargar la cabecera.");
                return response.text();
            })
            .then(html => {
                headerPlaceholder.innerHTML = html;

                updateIUser();
            })
            .catch(error => console.error("Error inyectando el layout:", error));
    } else {
        updateIUser();
    }
});
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

function updateIUser() {
    const cookieValue = getCookie("UserLogged");
    const isLogged = cookieValue !== null && cookieValue !== "";

    const navLogin = document.getElementById("navLogin");
    const navSellShirt = document.getElementById("navSellShirt");
    const navLogout = document.getElementById("navLogout");

    const btnSellShirtHome = document.getElementById("btnSellShirt");
    const btnSignInRegisterHome = document.getElementById("btnSignInRegister");

    if (isLogged) {
        if (navLogin) navLogin.style.display = "none";
        if (navSellShirt) navSellShirt.style.display = "block";
        if (navLogout) navLogout.style.display = "block";

        if (btnSellShirtHome) btnSellShirtHome.style.display = "inline-block";
        if (btnSignInRegisterHome) btnSignInRegisterHome.style.display = "none";
    } else {
        if (navLogin) navLogin.style.display = "block";
        if (navSellShirt) navSellShirt.style.display = "none";
        if (navLogout) navLogout.style.display = "none";

        if (btnSellShirtHome) btnSellShirtHome.style.display = "none";
        if (btnSignInRegisterHome) btnSignInRegisterHome.style.display = "inline-block";
    }

    const btnLogoutBtn = document.getElementById("btnLogoutBtn");
    if (btnLogoutBtn) {
        btnLogoutBtn.addEventListener("click", () => {
            document.cookie = "UserLogged=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
            window.location.href = "/";
        });
    }
}