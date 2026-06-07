export function getAccessToken(): string {
  const raw =
    localStorage.getItem("accessToken") ||
    localStorage.getItem("token") ||
    localStorage.getItem("jwt") ||
    localStorage.getItem("authToken") ||
    "";

  return raw
    .replace(/^Bearer\s+/i, "")
    .replace(/^"(.+)"$/, "$1")
    .trim();
}

export function getCurrentLogin(): string {
  return (
    localStorage.getItem("login") ||
    localStorage.getItem("userLogin") ||
    localStorage.getItem("organizationLogin") ||
    localStorage.getItem("loginOrganization") ||
    ""
  ).replace(/^"(.+)"$/, "$1");
}

export function authHeaders(): HeadersInit {
  const token = getAccessToken();

  if (!token) return {};

  return { Authorization: `Bearer ${token}` };
}
