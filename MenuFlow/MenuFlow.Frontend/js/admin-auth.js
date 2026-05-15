const ADMIN_TOKEN_KEY = "menuflow_admin_token";
const ADMIN_USER_KEY = "menuflow_admin_user";

const loginForm = document.getElementById("admin-login-form");
const loginButton = document.getElementById("login-button");
const loginError = document.getElementById("login-error");

document.addEventListener("DOMContentLoaded", () => {
  const token = localStorage.getItem(ADMIN_TOKEN_KEY);

  if (token && window.location.pathname.includes("/admin/login.html")) {
    window.location.href = "./dashboard.html";
    return;
  }

  if (loginForm) {
    loginForm.addEventListener("submit", handleLogin);
  }
});

async function handleLogin(event) {
  event.preventDefault();

  hideLoginError();

  const email = document.getElementById("email").value.trim();
  const senha = document.getElementById("senha").value.trim();

  try {
    setLoginLoading(true);

    const response = await apiPost("/auth/login", {
      email,
      senha
    });

    if (!response.sucesso) {
      showLoginError(response.mensagem || "Não foi possível fazer login.");
      return;
    }

    const authData = response.dados;

    localStorage.setItem(ADMIN_TOKEN_KEY, authData.token);
    localStorage.setItem(ADMIN_USER_KEY, JSON.stringify({
      nome: authData.nome,
      email: authData.email,
      role: authData.role
    }));

    window.location.href = "./dashboard.html";
  } catch (error) {
    showLoginError("Erro ao conectar com a API. Verifique se o backend está rodando.");
  } finally {
    setLoginLoading(false);
  }
}

function getAdminToken() {
  return localStorage.getItem(ADMIN_TOKEN_KEY);
}

function getAdminUser() {
  const user = localStorage.getItem(ADMIN_USER_KEY);
  return user ? JSON.parse(user) : null;
}

function requireAdminAuth() {
  const token = getAdminToken();

  if (!token) {
    window.location.href = "./login.html";
  }
}

function logoutAdmin() {
  localStorage.removeItem(ADMIN_TOKEN_KEY);
  localStorage.removeItem(ADMIN_USER_KEY);
  window.location.href = "./login.html";
}

function showLoginError(message) {
  loginError.textContent = message;
  loginError.classList.remove("hidden");
}

function hideLoginError() {
  loginError.textContent = "";
  loginError.classList.add("hidden");
}

function setLoginLoading(isLoading) {
  loginButton.disabled = isLoading;
  loginButton.textContent = isLoading ? "Entrando..." : "Entrar";
}