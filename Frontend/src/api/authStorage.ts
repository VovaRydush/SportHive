import type { AuthResponse } from "./authTypes";

export function saveAuth(data: AuthResponse) {
  const token = data.accessToken ?? data.token;

  if (token) localStorage.setItem("accessToken", token);
  if (data.refreshToken) localStorage.setItem("refreshToken", data.refreshToken);
  if (data.login) localStorage.setItem("login", data.login);
  if (data.email) localStorage.setItem("email", data.email);
  if (data.role) localStorage.setItem("role", data.role);
}

export function clearAuth() {
  localStorage.removeItem("accessToken");
  localStorage.removeItem("refreshToken");
  localStorage.removeItem("login");
  localStorage.removeItem("email");
  localStorage.removeItem("role");
}

export function isAuthorized() {
  return Boolean(localStorage.getItem("accessToken"));
}

export function getCurrentUser() {
  return {
    token: localStorage.getItem("accessToken"),
    login: localStorage.getItem("login"),
    email: localStorage.getItem("email"),
    role: localStorage.getItem("role"),
  };
}