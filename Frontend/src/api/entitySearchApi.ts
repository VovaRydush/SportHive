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
      console.warn(`[entitySearchApi] ${response.status} ${baseUrl}${path}`);
      return null;
    }

    return (await response.json()) as T;
  } catch (error) {
    console.warn(`[entitySearchApi] request failed: ${baseUrl}${path}`, error);
    return null;
  }
}

function value(obj: any, keys: string[]) {
  for (const key of keys) {
    const v = obj?.[key];

    if (v !== undefined && v !== null && String(v).trim() !== "") {
      return String(v);
    }
  }

  return "";
}

function normalizePhoto(input: unknown) {
  if (!input) return undefined;

  const text = String(input);

  if (text.startsWith("http") || text.startsWith("data:")) return text;

  if (text.length > 80 && !text.includes("/") && !text.includes("\\")) {
    return `data:image/png;base64,${text}`;
  }

  return text;
}

function normalize(type: PickerEntityType, item: any): PickerEntity {
  const id =
    value(item, ["login", "Login", "id", "Id", "teamName", "TeamName", "nameTeam", "NameTeam"]) ||
    crypto.randomUUID();

  const fullName =
    value(item, ["fullName", "FullName", "name", "Name", "teamName", "TeamName", "nameTeam", "NameTeam"]) ||
    [value(item, ["firsName", "FirsName", "firstName", "FirstName"]), value(item, ["lastName", "LastName"])]
      .filter(Boolean)
      .join(" ") ||
    id;

  const subtitle = [
    value(item, ["typeSport", "TypeSport"]),
    value(item, ["role", "Role"]),
    id !== fullName ? `login: ${id}` : "",
  ].filter(Boolean).join(" · ");

  return {
    id,
    title: fullName,
    subtitle,
    type,
    photo: normalizePhoto(value(item, [
      "profilePhoto",
      "ProfilePhoto",
      "profilePhotoPath",
      "ProfilePhotoPath",
      "photo",
      "Photo",
      "teamPhoto",
      "TeamPhoto"
    ])),
    raw: item,
  };
}

function onlyMatching(items: PickerEntity[], query: string) {
  const q = query.toLowerCase();

  return items.filter(item =>
    `${item.id} ${item.title} ${item.subtitle || ""}`.toLowerCase().includes(q)
  );
}

export const entitySearchApi = {
  async search(type: PickerEntityType, query: string): Promise<PickerEntity[]> {
    const text = query.trim();

    if (!text) return [];

    const q = encodeURIComponent(text);
    const org = encodeURIComponent(getOrgLogin());

    if (type === "trainer") {
      if (!org) return [];

      const data =
        (await tryGet<any[]>(AUTH_API_URL, `/organization-linked/trainers?loginOrganization=${org}&query=${q}`)) ??
        [];

      return onlyMatching(data.map(x => normalize("trainer", x)), text);
    }

    if (type === "athlete") {
      const orgAthletes = org
        ? await tryGet<any[]>(AUTH_API_URL, `/organization-linked/athletes?loginOrganization=${org}&query=${q}`)
        : null;

      if (orgAthletes?.length) {
        return onlyMatching(orgAthletes.map(x => normalize("athlete", x)), text);
      }

      // Fallback for old project state where OrganizationAthlete is not filled yet.
      const globalAthletes =
        (await tryGet<any[]>(AUTH_API_URL, `/get-search-athlete?FullName=${q}`)) ??
        (await tryGet<any[]>(AUTH_API_URL, `/get-search-athlete/${q}`)) ??
        [];

      return onlyMatching(globalAthletes.map(x => normalize("athlete", x)), text);
    }

    if (type === "judge") {
      if (!org) return [];

      const data =
        (await tryGet<any[]>(AUTH_API_URL, `/organization-linked/judges?loginOrganization=${org}&query=${q}`)) ??
        [];

      return onlyMatching(data.map(x => normalize("judge", x)), text);
    }

    if (type === "team") {
      const linked =
        org
          ? await tryGet<any[]>(AUTH_API_URL, `/organization-linked/teams?loginOrganization=${org}&query=${q}`)
          : null;

      if (linked?.length) {
        return onlyMatching(linked.map(x => normalize("team", x)), text);
      }

      const data =
        (await tryGet<any[]>(COMMAND_API_URL, `/teams/search?query=${q}`)) ??
        (await tryGet<any[]>(COMMAND_API_URL, `/get-search-team?NameTeam=${q}`)) ??
        [];

      return onlyMatching(data.map(x => normalize("team", x)), text);
    }

    if (type === "organization") {
      const data =
        (await tryGet<any[]>(AUTH_API_URL, `/organization/search?query=${q}`)) ??
        (await tryGet<any[]>(AUTH_API_URL, `/get-search-organization?Name=${q}`)) ??
        [];

      return onlyMatching(data.map(x => normalize("organization", x)), text);
    }

    const data =
      (await tryGet<any[]>(AUTH_API_URL, `/users/search?query=${q}`)) ??
      [];

    return onlyMatching(data.map(x => normalize("user", x)), text);
  },
};
