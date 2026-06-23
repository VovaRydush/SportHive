export type UserRole = "Guest" | "Athlete" | "Trainer" | "Judge" | "Organization";

export function cleanStorage(value: string | null): string {
  return (value || "").replace(/^"(.+)"$/, "$1").trim();
}

export function getAccessToken(): string {
  return (
    cleanStorage(localStorage.getItem("accessToken")) ||
    cleanStorage(localStorage.getItem("token")) ||
    cleanStorage(localStorage.getItem("jwt")) ||
    cleanStorage(localStorage.getItem("authToken"))
  ).replace(/^Bearer\s+/i, "");
}

export function getCurrentLogin(): string {
  return (
    cleanStorage(localStorage.getItem("login")) ||
    cleanStorage(localStorage.getItem("userLogin")) ||
    cleanStorage(localStorage.getItem("organizationLogin")) ||
    cleanStorage(localStorage.getItem("loginOrganization"))
  );
}

export function getCurrentRole(): UserRole {
  const raw = (
    cleanStorage(localStorage.getItem("userRole")) ||
    cleanStorage(localStorage.getItem("role")) ||
    cleanStorage(localStorage.getItem("Role"))
  ).toLowerCase();

  if (raw.includes("organization") || raw.includes("орган")) return "Organization";
  if (raw.includes("trainer") || raw.includes("трен")) return "Trainer";
  if (raw.includes("judge") || raw.includes("суд")) return "Judge";
  if (raw.includes("athlete") || raw.includes("спорт")) return "Athlete";

  return getAccessToken() ? "Athlete" : "Guest";
}

export function authHeaders(): HeadersInit {
  const token = getAccessToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
}

export function clearAuthStorage(): void {
  [
    "accessToken",
    "token",
    "jwt",
    "authToken",
    "userRole",
    "role",
    "Role",
    "login",
    "userLogin",
    "organizationLogin",
    "loginOrganization",
  ].forEach(key => localStorage.removeItem(key));
}
