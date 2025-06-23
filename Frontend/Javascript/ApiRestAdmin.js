arrayCentrosMedicos = [];
arrayMedicos = [];
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

//METODOS PARA ADMINITRADOR
document.getElementById("userName").textContent = decodeJwt(
  localStorage.getItem("token")
).email;

document.getElementById("logoutBtn").addEventListener("click", function () {
  localStorage.removeItem("token");
  window.location.href = "/Frontend/index.html";
});

async function getCentroMedicos() {
  try {
    const response = await fetch("https://localhost:7298/api/CentrosMedicos", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    });
    if (!response.ok) {
      throw new Error("Error en la solicitud: " + response.statusText);
    }
    arrayCentrosMedicos = await response.json();
    document.getElementById("centrosCount").textContent =
      arrayCentrosMedicos.length;
    renderCentrosMedicos(); // Renderizar centros médicos en la tabla
  } catch (error) {
    console.error("Error al obtener los centros médicos:", error);
    throw new Error("Error al obtener los centros médicos: " + error.message);
  }
}
// Renderizar centros médicos en la tabla
function renderCentrosMedicos() {
  const tbody = document.getElementById("centrosTableBody");
  tbody.innerHTML = ""; // Limpiar tabla
  arrayCentrosMedicos.forEach((centro) => {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td>${centro.nombre}</td>
      <td>${centro.direccion}</td>
      <td>${centro.ciudad.nombre}</td>
      <td>
        <button class="btn btn-warning btn-sm" onclick="editCentro(${centro.id})">
          <i class="bi bi-pencil"></i>
        </button>
        <button class="btn btn-danger btn-sm" onclick="deleteCentro(${centro.id})">
          <i class="bi bi-trash"></i>
        </button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

// Crear centro médico
document
  .getElementById("formCrearCentro")
  .addEventListener("submit", async function (event) {
    event.preventDefault();
    const nombre = document.getElementById("nombreCentro").value;
    const direccion = document.getElementById("direccionCentro").value;
    const ciudadId = document.getElementById("ciudadIdCentro").value;

    try {
      const response = await fetch(
        "https://localhost:7298/api/CentrosMedicos",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({
            nombre,
            direccion,
            ciudadIdCiudad: parseInt(ciudadId),
          }),
        }
      );
      if (!response.ok) {
        throw new Error(
          "Error al crear el centro médico: " + response.statusText
        );
      }
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("addCenterModal")
      );
      modal.hide();
      await getCentrosMedicos(); // Actualizar tabla
      document.getElementById("formCrearCentro").reset();
    } catch (error) {
      console.error("Error al crear centro médico:", error);
      alert("Error al crear centro médico: " + error.message);
    }
  });

// Editar centro médico
window.editCentro = async function (id) {
  const centro = arrayCentrosMedicos.find((c) => c.id === id);
  if (centro) {
    document.getElementById("editarCentroId").value = centro.id;
    document.getElementById("editarNombreCentro").value = centro.nombre;
    document.getElementById("editarDireccionCentro").value = centro.direccion;
    document.getElementById("editarCiudadIdCentro").value =
      centro.ciudadIdCiudad;
    const modal = new bootstrap.Modal(
      document.getElementById("editCenterModal")
    );
    modal.show();
  }
};

document
  .getElementById("formEditarCentro")
  .addEventListener("submit", async function (event) {
    event.preventDefault();
    const id = document.getElementById("editarCentroId").value;
    const nombre = document.getElementById("editarNombreCentro").value;
    const direccion = document.getElementById("editarDireccionCentro").value;
    const ciudadId = document.getElementById("editarCiudadIdCentro").value;

    try {
      const response = await fetch(
        `https://localhost:7298/api/CentrosMedicos/${id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({
            id,
            nombre,
            direccion,
            ciudadIdCiudad: parseInt(ciudadId),
          }),
        }
      );
      if (!response.ok) {
        throw new Error(
          "Error al actualizar el centro médico: " + response.statusText
        );
      }
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("editCenterModal")
      );
      modal.hide();
      await getCentrosMedicos(); // Actualizar tabla
    } catch (error) {
      console.error("Error al actualizar centro médico:", error);
      alert("Error al actualizar centro médico: " + error.message);
    }
  });

// Eliminar centro médico
window.deleteCentro = async function (id) {
  if (confirm("¿Estás seguro de eliminar este centro médico?")) {
    try {
      const response = await fetch(
        `https://localhost:7298/api/CentrosMedicos/${id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      if (!response.ok) {
        throw new Error(
          "Error al eliminar el centro médico: " + response.statusText
        );
      }
      getCentrosMedicos(); // Actualizar tabla
    } catch (error) {
      console.error("Error al eliminar centro médico:", error);
      alert("Error al eliminar centro médico: " + error.message);
    }
  }
};

// Navegación entre secciones
document.querySelectorAll("#navTabs .nav-link").forEach((link) => {
  link.addEventListener("click", function (e) {
    e.preventDefault();
    const sectionId = this.getAttribute("data-section");
    document.querySelectorAll(".content-section").forEach((section) => {
      section.classList.remove("active");
    });
    document.getElementById(sectionId).classList.add("active");
    document.querySelectorAll("#navTabs .nav-link").forEach((nav) => {
      nav.classList.remove("active");
    });
    this.classList.add("active");
  });
});

async function getMedicos() {
  try {
    const response = await fetch("https://localhost:7298/api/Medicos", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    });
    arrayMedicos = await response.json();
    document.getElementById("medicosCount").textContent = arrayMedicos.length;
    renderMedicos(); // Renderizar médicos en la tabla
  } catch (error) {
    console.error("Error al obtener los médicos:", error);
    throw new Error("Error al obtener los médicos: " + error.message);
  }
}
// Renderizar médicos en la tabla
function renderMedicos() {
  const tbody = document.querySelector("#doctors table tbody"); // Selecciona el tbody de la tabla de médicos
  tbody.innerHTML = "";
  arrayMedicos.forEach((medico) => {
    // Buscar el nombre del centro médico asociado
    const tr = document.createElement("tr");

    tr.innerHTML = `
      <td>${medico.nombre} ${medico.apellido}</td>
      <td>${medico.especialidad.nombre}</td>
      <td>${medico.centroMedico.nombre}</td>
      <td>${medico.correo}</td>
      <td>
        <button class="btn btn-warning btn-sm" onclick="editMedico(${medico.id})">
          <i class="bi bi-pencil"></i>
        </button>
        <button class="btn btn-danger btn-sm" onclick="deleteMedico(${medico.id})">
          <i class="bi bi-trash"></i>
        </button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

getCentroMedicos();
getMedicos();
