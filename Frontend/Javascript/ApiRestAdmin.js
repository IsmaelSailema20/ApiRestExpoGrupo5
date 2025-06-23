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

// METODOS PARA ADMINISTRADOR
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
    document.getElementById("centrosMedQuito").textContent =
      arrayCentrosMedicos.filter(
        (centro) => centro.ciudad.nombre === "Quito"
      ).length;
    document.getElementById("centrosMedGuayaquil").textContent =
      arrayCentrosMedicos.filter(
        (centro) => centro.ciudad.nombre === "Guayaquil"
      ).length;
    document.getElementById("centrosMedCuenca").textContent =
      arrayCentrosMedicos.filter(
        (centro) => centro.ciudad.nombre === "Cuenca"
      ).length;
    renderCentrosMedicos(); // Renderizar centros médicos en la tabla
    populateCentroMedicoSelects(); // Poblar los select de centros médicos
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

// Poblar los select de centros médicos
function populateCentroMedicoSelects() {
  const selects = [
    document.getElementById("idCentroMedico"),
    document.getElementById("editarIdCentroMedico"),
  ];
  selects.forEach((select) => {
    select.innerHTML = '<option value="0">Seleccionar Centro Médico</option>';
    arrayCentrosMedicos.forEach((centro) => {
      const option = document.createElement("option");
      option.value = centro.id;
      option.textContent = centro.nombre;
      select.appendChild(option);
    });
  });
}

// Crear centro médico
document
  .getElementById("formCrearCentro")
  .addEventListener("submit", async function (event) {
    event.preventDefault();
    const nombre = document.getElementById("nombreCentro").value;
    const direccion = document.getElementById("direccionCentro").value;
    const ciudad_id = parseInt(document.getElementById("ciudadIdCentro").value);

    if (ciudad_id === 0 || isNaN(ciudad_id)) {
      alert("Por favor, seleccione una ciudad válida.");
      return;
    }

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
            ciudad_id,
          }),
        }
      );
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          `Error al crear el centro médico: ${
            errorData.message || response.statusText
          }`
        );
      }
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("addCenterModal")
      );
      modal.hide();
      await getCentroMedicos();
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
    document.getElementById("editarCiudadIdCentro").value = centro.ciudad_id; // Corregido para seleccionar la ciudad correcta
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
    const ciudad_id = parseInt(
      document.getElementById("editarCiudadIdCentro").value
    );

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
            ciudad_id,
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
      await getCentroMedicos();
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
      getCentroMedicos();
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
    renderMedicos();
    populateCentroMedicoSelects(); // Asegurar que los select de médicos estén poblados
  } catch (error) {
    console.error("Error al obtener los médicos:", error);
    throw new Error("Error al obtener los médicos: " + error.message);
  }
}

// Renderizar médicos en la tabla
function renderMedicos() {
  const tbody = document.querySelector("#doctors table tbody");
  tbody.innerHTML = "";
  arrayMedicos.forEach((medico) => {
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

// Crear médico
document
  .getElementById("formCrearMedico")
  .addEventListener("submit", async function (event) {
    event.preventDefault();
    const nombre = document.getElementById("nombreMedico").value;
    const apellido = document.getElementById("apellidoMedico").value;
    const correo = document.getElementById("correoMedico").value;
    const id_especialidad = parseInt(
      document.getElementById("idEspecialidad").value
    );
    const id_centro_medico = parseInt(
      document.getElementById("idCentroMedico").value
    );
    const rol = "medico";

    if (
      id_especialidad === 0 ||
      id_centro_medico === 0 ||
      !nombre ||
      !apellido ||
      !correo ||
      !rol
    ) {
      alert(
        "Por favor, complete todos los campos y seleccione valores válidos."
      );
      return;
    }

    try {
      const response = await fetch("https://localhost:7298/api/Medicos", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify({
          nombre,
          apellido,
          correo,
          id_especialidad,
          id_centro_medico,
          rol,
        }),
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          `Error al crear el médico: ${
            errorData.message || response.statusText
          }`
        );
      }
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("addDoctorModal")
      );
      modal.hide();
      await getMedicos();
      document.getElementById("formCrearMedico").reset();
    } catch (error) {
      console.error("Error al crear médico:", error);
      alert("Error al crear médico: " + error.message);
    }
  });

// Editar médico
window.editMedico = async function (id) {
  const medico = arrayMedicos.find((m) => m.id === id);
  if (medico) {
    document.getElementById("editarMedicoId").value = medico.id;
    document.getElementById("editarNombreMedico").value = medico.nombre;
    document.getElementById("editarApellidoMedico").value = medico.apellido;
    document.getElementById("editarCorreoMedico").value = medico.correo;
    document.getElementById("editarIdEspecialidad").value =
      medico.id_especialidad;
    document.getElementById("editarIdCentroMedico").value =
      medico.id_centro_medico;
    const modal = new bootstrap.Modal(
      document.getElementById("editDoctorModal")
    );
    modal.show();
  }
};

document
  .getElementById("formEditarMedico")
  .addEventListener("submit", async function (event) {
    event.preventDefault();
    const id = document.getElementById("editarMedicoId").value;
    const nombre = document.getElementById("editarNombreMedico").value;
    const apellido = document.getElementById("editarApellidoMedico").value;
    const correo = document.getElementById("editarCorreoMedico").value;
    const id_especialidad = parseInt(
      document.getElementById("editarIdEspecialidad").value
    );
    const id_centro_medico = parseInt(
      document.getElementById("editarIdCentroMedico").value
    );
    const rol = "medico";

    try {
      const response = await fetch(`https://localhost:7298/api/Medicos/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify({
          id,
          nombre,
          apellido,
          correo,
          id_especialidad,
          id_centro_medico,
          rol,
        }),
      });
      if (!response.ok) {
        throw new Error(
          "Error al actualizar el médico: " + response.statusText
        );
      }
      const modal = bootstrap.Modal.getInstance(
        document.getElementById("editDoctorModal")
      );
      modal.hide();
      await getMedicos();
    } catch (error) {
      console.error("Error al actualizar médico:", error);
      alert("Error al actualizar médico: " + error.message);
    }
  });

// Eliminar médico
window.deleteMedico = async function (id) {
  if (confirm("¿Estás seguro de eliminar este médico?")) {
    try {
      const response = await fetch(`https://localhost:7298/api/Medicos/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      if (!response.ok) {
        throw new Error("Error al eliminar el médico: " + response.statusText);
      }
      await getMedicos();
    } catch (error) {
      console.error("Error al eliminar médico:", error);
      alert("Error al eliminar médico: " + error.message);
    }
  }
};

getCentroMedicos();
getMedicos();
