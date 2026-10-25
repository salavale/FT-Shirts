document.addEventListener("DOMContentLoaded", () => {
    const registerForm = document.getElementById("registerForm");

    if (registerForm) {
        registerForm.addEventListener("submit", (e) => {
            e.preventDefault();

            const username = document.getElementById("newUsername").value.trim();
            const password = document.getElementById("newPassword").value.trim();

            const requestBody = {
                username: username,
                password: password
            };

            fetch('/api/user/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            })
                .then(response => {
                    if (response.ok) {
                        window.location.href = "/user/UserLogin.html";
                    } else if (response.status === 409) {
                    } else {
                        throw new Error("Registration failed.");
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                });
        });
    }
});