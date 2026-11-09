// Dirección del backend (proyecto FTShirts.Web), que arranca en el puerto 5000.
// Se usa el mismo host con el que se ha abierto el frontend (localhost o 127.0.0.1):
// así el navegador trata frontend y API como el mismo "sitio" y comparte la cookie
// de sesión UserLogged entre los dos. Si cambias el puerto del backend en
// FTShirts.Web/Properties/launchSettings.json, cámbialo también aquí.
const API_PORT = 5000;
const API_URL = `http://${window.location.hostname || "localhost"}:${API_PORT}`;
