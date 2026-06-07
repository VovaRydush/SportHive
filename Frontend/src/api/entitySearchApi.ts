import { apiRequest, AUTH_API_URL, COMMAND_API_URL, EVENT_API_URL } from "./http";
import type { PickerEntity, PickerEntityType } from "./entityPickerTypes";

function normalizePhoto(value: unknown): string | undefined {
  if (!value) return undefined;
  const text = String(value);
  if (text.startsWith("data:") || text.startsWith("http")) return text;
  if (text.length > 80 && !text.includes("/") && !text.includes("\\")) return `data:image/png;base64,${text}`;
  return text;
}

function getString(obj: any, keys: string[]) {
  for (const key of keys) {
    if (obj && obj[key] !== undefined && obj[key] !== null && String(obj[key]).trim() !== "") {
      return String(obj[key]);
    }
  }

  return "";
}

function normalizeEntity(type: PickerEntityType, item: any): PickerEntity {
  const login = getString(item, ["login", "Login", "id", "Id", "userLogin", "UserLogin"]);
  const name =
    getString(item, ["fullName", "FullName", "name", "Name", "teamName", "TeamName", "nameTeam", "NameTeam", "nameOrganization", "NameOrganization"]) ||
    [getString(item, ["firsName", "FirsName", "firstName", "FirstName"]), getString(item, ["lastName", "LastName"])]
      .filter(Boolean)
      .join(" ") ||
    login ||
    "Без назви";

  const subtitleParts = [
    getString(item, ["typeSport", "TypeSport", "sport", "Sport"]),
    getString(item, ["role", "Role"]),
    login && login !== name ? `login: ${login}` : "",
  ].filter(Boolean);

  return {
    id: login || name,
    title: name,
    subtitle: subtitleParts.join(" · "),
    photo: normalizePhoto(getString(item, ["profilePhotoPath", "ProfilePhotoPath", "profilePhoto", "ProfilePhoto", "photo", "Photo", "teamPhoto", "TeamPhoto"])),
    type,
    raw: item,
  };
}

async function safeRequest<T>(path: string, options: any): Promise<T | null> {
  try {
    return await apiRequest<T>(path, options);
  } catch {
    return null;
  }
}

export const entitySearchApi = {
  async search(type: PickerEntityType, query: string): Promise<PickerEntity[]> {
    const q = encodeURIComponent(query.trim());

    if (!q) return [];

    if (type === "athlete") {
      const data =
        await safeRequest<any[]>(`/get-search-athlete/${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
        await safeRequest<any[]>(`/get-search-athlete?FullName=${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
        [];

      return data.map(item => normalizeEntity("athlete", item));
    }

    if (type === "team") {
      const data =
        await safeRequest<any[]>(`/teams/search?query=${q}`, { baseUrl: COMMAND_API_URL, method: "GET", auth: false }) ??
        await safeRequest<any[]>(`/get-search-team?NameTeam=${q}`, { baseUrl: COMMAND_API_URL, method: "GET", auth: false }) ??
        await safeRequest<any[]>(`/teams`, { baseUrl: COMMAND_API_URL, method: "GET", auth: false }) ??
        [];

      return data
        .map(item => normalizeEntity("team", item))
        .filter(item => `${item.title} ${item.subtitle || ""}`.toLowerCase().includes(query.toLowerCase()));
    }

    if (type === "trainer") {
      const data =
        await safeRequest<any[]>(`/users/search?role=Trainer&query=${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
        await safeRequest<any[]>(`/get-search-trainer?FullName=${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
        [];

      return data.map(item => normalizeEntity("trainer", item));
    }

    if (type === "judge") {
      const data =
        await safeRequest<any[]>(`/users/search?role=Judge&query=${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
        await safeRequest<any[]>(`/get-search-judge?FullName=${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
        [];

      return data.map(item => normalizeEntity("judge", item));
    }

    if (type === "organization") {
      const data =
        await safeRequest<any[]>(`/users/search?role=Organization&query=${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
        await safeRequest<any[]>(`/get-search-organization?Name=${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
        [];

      return data.map(item => normalizeEntity("organization", item));
    }

    const data =
      await safeRequest<any[]>(`/users/search?query=${q}`, { baseUrl: AUTH_API_URL, method: "GET", auth: false }) ??
      [];

    return data.map(item => normalizeEntity("user", item));
  },
};
