document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById("loginForm");

    if (loginForm) {
        loginForm.addEventListener("submit", (e) => {
            e.preventDefault();

            const username = document.getElementById("username").value.trim();
            const password = document.getElementById("password").value.trim();

            const requestBody = {
                username: username,
                password: password
            };

            fetch('/api/user/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            })
                .then(response => {
                    if (response.ok) {
                        window.location.href = "/catalogue/Index.html";
                    } else if (response.status === 401) {

                    } else {
                        throw new Error("Login failed.");
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    alert("An error occurred during login.");
                });
        });
    }
});