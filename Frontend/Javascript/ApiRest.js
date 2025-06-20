// Funcionalidad de login
document.addEventListener("DOMContentLoaded", function () {
  // Login functionality
  const loginForm = document.getElementById("loginForm");
  if (loginForm) {
    loginForm.addEventListener("submit", function (e) {
      e.preventDefault();

      const username = document.getElementById("username").value;
      const password = document.getElementById("password").value;

      if (username === "admin" && password === "admin123") {
        window.location.href = "admin.html";
      } else if (username === "doctor" && password === "doctor123") {
        window.location.href = "doctor.html";
      } else {
        alert(
          'Credenciales incorrectas. Usa "admin/admin123" o "doctor/doctor123"'
        );
      }
    });
  }

  // Navigation functionality
  const navLinks = document.querySelectorAll(".nav-link");
  const contentSections = document.querySelectorAll(".content-section");

  if (navLinks.length > 0) {
    navLinks.forEach((link) => {
      link.addEventListener("click", function (e) {
        e.preventDefault();

        const targetSection = this.dataset.section;

        // Update active nav link
        navLinks.forEach((l) => l.classList.remove("active"));
        this.classList.add("active");

        // Show target section
        contentSections.forEach((section) => {
          section.classList.remove("active");
          if (section.id === targetSection) {
            section.classList.add("active");
          }
        });
      });
    });
  }

  // Logout functionality
  const logoutBtn = document.getElementById("logoutBtn");
  if (logoutBtn) {
    logoutBtn.addEventListener("click", function () {
      window.location.href = "index.html";
    });
  }
});
