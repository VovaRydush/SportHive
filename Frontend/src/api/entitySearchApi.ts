import { AUTH_API_URL, COMMAND_API_URL } from "./http";
import type { PickerEntity, PickerEntityType } from "./entityPickerTypes";

function clean(value: string | null) {
  return (value || "").replace(/^"(.+)"$/, "$1").trim();
}

function getOrgLogin() {
  return (
    clean(localStorage.getItem("organizationLogin")) ||
    clean(localStorage.getItem("loginOrganization")) ||
    clean(localStorage.getItem("login")) ||
    clean(localStorage.getItem("userLogin"))
  );
}

function getToken() {
  return (
    clean(localStorage.getItem("accessToken")) ||
    clean(localStorage.getItem("token")) ||
    clean(localStorage.getItem("jwt")) ||
    clean(localStorage.getItem("authToken"))
  ).replace(/^Bearer\s+/i, "");
}

async function tryGet<T>(baseUrl: string, path: string): Promise<T | null> {
  try {
    const token = getToken();

    const response = await fetch(`${baseUrl}${path}`, {
      method: "GET",
      headers: {
        Accept: "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });

    if (!response.ok) {
      console.warn(`[entitySearchApi] ${response.status}: ${baseUrl}${path}`);
      return null;
    }

    return (await response.json()) as T;
  } catch (error) {
    console.warn(`[entitySearchApi] failed: ${baseUrl}${path}`, error);
    return null;
  }
}

function s(obj: any, keys: string[]) {
  for (const key of keys) {
    const value = obj?.[key];

    if (value !== undefined && value !== null && String(value).trim() !== "") {
      return String(value);
    }
  }

  return "";
}

function photo(value: unknown) {
  if (!value) return undefined;

  const text = String(value);

  if (text.startsWith("http") || text.startsWith("data:")) return text;
  if (text.length > 80 && !text.includes("/") && !text.includes("\\")) return `data:image/png;base64,${text}`;

  return text;
}

function normalize(type: PickerEntityType, item: any): PickerEntity {
  const id =
    s(item, ["login", "Login", "id", "Id", "teamName", "TeamName", "nameTeam", "NameTeam"]) ||
    crypto.randomUUID();

  const title =
    s(item, ["fullName", "FullName", "name", "Name", "teamName", "TeamName", "nameTeam", "NameTeam"]) ||
    [s(item, ["firsName", "FirsName", "firstName", "FirstName"]), s(item, ["lastName", "LastName"])]
      .filter(Boolean)
      .join(" ") ||
    id;

  const subtitle = [
    s(item, ["typeSport", "TypeSport"]),
    s(item, ["role", "Role"]),
    s(item, ["source", "Source"]),
    id !== title ? `login: ${id}` : "",
  ].filter(Boolean).join(" · ");

  return {
    id,
    title,
    subtitle,
    type,
    photo: photo(s(item, ["profilePhoto", "ProfilePhoto", "profilePhotoPath", "ProfilePhotoPath", "photo", "Photo", "teamPhoto", "TeamPhoto"])),
    raw: item,
  };
}

function unique(items: PickerEntity[]) {
  const map = new Map<string, PickerEntity>();
  items.forEach(item => {
    if (!map.has(item.id)) map.set(item.id, item);
  });
  return Array.from(map.values());
}

function match(items: PickerEntity[], query: string) {
  const q = query.toLowerCase();

  return items.filter(item =>
    `${item.id} ${item.title} ${item.subtitle || ""}`.toLowerCase().includes(q)
  );
}

function asArray<T>(value: T[] | T | null | undefined): T[] {
  if (!value) return [];
  return Array.isArray(value) ? value : [value];
}

export const entitySearchApi = {
  async search(type: PickerEntityType, query: string): Promise<PickerEntity[]> {
    const text = query.trim();
    if (!text) return [];

    const q = encodeURIComponent(text);
    const org = encodeURIComponent(getOrgLogin());

    if (type === "trainer") {
      const linked = org
        ? await tryGet<any[]>(AUTH_API_URL, `/organization-linked/trainers?loginOrganization=${org}&query=${q}`)
        : [];

      return match(unique(asArray(linked).map(x => normalize("trainer", x))), text);
    }

    if (type === "athlete") {
      const linked = org
        ? await tryGet<any[]>(AUTH_API_URL, `/organization-linked/athletes?loginOrganization=${org}&query=${q}`)
        : [];

      const global1 = await tryGet<any[] | any>(AUTH_API_URL, `/get-search-athlete?FullName=${q}`);
      const global2 = await tryGet<any[] | any>(AUTH_API_URL, `/get-search-athlete/${q}`);

      const merged = [
        ...asArray(linked).map(x => normalize("athlete", x)),
        ...asArray(global1).map(x => normalize("athlete", x)),
        ...asArray(global2).map(x => normalize("athlete", x)),
      ];

      return match(unique(merged), text);
    }

    if (type === "judge") {
      const linked = org
        ? await tryGet<any[]>(AUTH_API_URL, `/organization-linked/judges?loginOrganization=${org}&query=${q}`)
        : [];

      return match(unique(asArray(linked).map(x => normalize("judge", x))), text);
    }

    if (type === "team") {
      const linked = org
        ? await tryGet<any[]>(AUTH_API_URL, `/organization-linked/teams?loginOrganization=${org}&query=${q}`)
        : [];

      const global =
        await tryGet<any[]>(COMMAND_API_URL, `/teams/search?query=${q}`) ??
        await tryGet<any[]>(COMMAND_API_URL, `/get-search-team?NameTeam=${q}`) ??
        [];

      return match(unique([...asArray(linked), ...asArray(global)].map(x => normalize("team", x))), text);
    }

    if (type === "organization") {
      const data =
        await tryGet<any[]>(AUTH_API_URL, `/organization/search?query=${q}`) ??
        await tryGet<any[]>(AUTH_API_URL, `/get-search-organization?Name=${q}`) ??
        [];

      return match(unique(asArray(data).map(x => normalize("organization", x))), text);
    }

    const users = await tryGet<any[]>(AUTH_API_URL, `/users/search?query=${q}`) ?? [];
    return match(unique(asArray(users).map(x => normalize("user", x))), text);
  },
};
