import type { AuthResponse } from "./authTypes";

export function saveAuth(data: AuthResponse) {
  const token = data.token || data.accessToken;

  if (token) {
    localStorage.setItem("accessToken", token);
  }

  if (data.refreshToken) {
    localStorage.setItem("refreshToken", data.refreshToken);
  }

  if (data.role) {
    localStorage.setItem("userRole", data.role);
    localStorage.setItem("role", data.role);
  }

  if (data.login) {
    localStorage.setItem("login", data.login);
  }

  if (data.email) {
    localStorage.setItem("email", data.email);
  }
}

export function clearAuth() {
  localStorage.removeItem("accessToken");
  localStorage.removeItem("refreshToken");
  localStorage.removeItem("userRole");
  localStorage.removeItem("role");
  localStorage.removeItem("login");
  localStorage.removeItem("email");
}

export function getCurrentUser() {
  return {
    token: localStorage.getItem("accessToken"),
    role: localStorage.getItem("userRole") || localStorage.getItem("role"),
    login: localStorage.getItem("login"),
    email: localStorage.getItem("email"),
  };
}

export function isAuthorized() {
  return Boolean(localStorage.getItem("accessToken"));
}
