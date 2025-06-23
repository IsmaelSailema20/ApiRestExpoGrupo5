// Función para decodificar el payload del JWT
function decodeJwt(token) {
  try {
    const payload = token.split(".")[1]; // Obtener el payload (segunda parte del JWT)
    const decoded = atob(payload); // Decodificar Base64
    return JSON.parse(decoded); // Parsear a JSON
  } catch (error) {
    throw new Error("Error al decodificar el token: " + error.message);
  }
}

async function login(email, password) {
  const mensaje = document.getElementById("errorAlert");
  mensaje.style.display = "none";

  try {
    const response = await fetch("https://localhost:7298/api/Usuarios/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email,
        password,
      }),
    });

    if (!response.ok) {
      mensaje.textContent = "Usuario o contraseña incorrectos";
      mensaje.style.display = "block";
      throw new Error("Error en la solicitud: " + response.statusText);
    }

    const data = await response.json();

    if (data.token) {
      localStorage.setItem("token", data.token);

      // Decodificar el JWT
      const decodedToken = decodeJwt(data.token);
      const rol =
        decodedToken.role ||
        decodedToken.roles ||
        decodedToken[
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ] ||
        decodedToken.Rol;

      if (!rol) {
        mensaje.textContent = "No se encontró el rol en el token";
        mensaje.style.display = "block";
        throw new Error("Rol no encontrado en el token");
      }

      // Verificar si es administrador
      if (rol.toLowerCase() === "admin") {
        window.location.href = "/Frontend/admin.html";
      } else {
        window.location.href = "/Frontend/doctor.html";
      }
      return true;
    } else {
      mensaje.textContent = "Token no recibido";
      mensaje.style.display = "block";
      throw new Error("Token no recibido");
    }
  } catch (error) {
    console.error("Error en la función login:", error);
    mensaje.textContent = error.message || "Error al iniciar sesión";
    mensaje.style.display = "block";
    return false;
  }
}

document
  .getElementById("loginForm")
  .addEventListener("submit", async function (event) {
    event.preventDefault();
    const usuario = document.getElementById("username").value;
    const contraseña = document.getElementById("password").value;
    await login(usuario, contraseña);
  });
