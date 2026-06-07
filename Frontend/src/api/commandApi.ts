import { apiRequest, COMMAND_API_URL } from "./http";
import type {
  NewSatatusAthlete,
  OrganizationTeamDto,
  TeamAthleteDto,
  TeamInfoDto,
  TeamModelDto,
} from "./commandTypes";

function firstFormValue(formData: FormData, keys: string[]): string {
  for (const key of keys) {
    const value = formData.get(key);

    if (value !== null && value !== undefined && String(value).trim() !== "") {
      return String(value);
    }
  }

  return "";
}

function firstFormFile(formData: FormData, keys: string[]): File | null {
  for (const key of keys) {
    const value = formData.get(key);

    if (value instanceof File) {
      return value;
    }
  }

  return null;
}

function appendIfExists(formData: FormData, key: string, value: unknown) {
  if (value !== undefined && value !== null && value !== "") {
    formData.append(key, value as string | Blob);
  }
}

function normalizeTeamInput(data: TeamModelDto | FormData): TeamModelDto & { loginOrganization?: string } {
  if (!(data instanceof FormData)) {
    const anyData = data as any;

    return {
      nameTeam: anyData.nameTeam ?? anyData.NameTeam ?? anyData.TeamName ?? "",
      loginTrainer: anyData.loginTrainer ?? anyData.LoginTrainer ?? "",
      typeSport: anyData.typeSport ?? anyData.TypeSport ?? "",
      photo: anyData.photo ?? anyData.Photo ?? null,
      athletsJson: anyData.athletsJson ?? anyData.AthletsJson,
      athlets: anyData.athlets ?? anyData.Athlets ?? [],
      loginOrganization:
        anyData.loginOrganization ??
        anyData.LoginOrganization ??
        anyData.organizationLogin ??
        "",
    };
  }

  return {
    nameTeam: firstFormValue(data, ["nameTeam", "NameTeam", "TeamName"]),
    loginTrainer: firstFormValue(data, ["loginTrainer", "LoginTrainer"]),
    typeSport: firstFormValue(data, ["typeSport", "TypeSport"]),
    photo: firstFormFile(data, ["photo", "Photo"]),
    athletsJson: firstFormValue(data, ["athletsJson", "AthletsJson"]),
    athlets: [],
    loginOrganization: firstFormValue(data, ["loginOrganization", "LoginOrganization", "organizationLogin"]),
  };
}

function createTeamFormData(data: TeamModelDto | FormData) {
  const normalized = normalizeTeamInput(data);
  const formData = new FormData();

  appendIfExists(formData, "NameTeam", normalized.nameTeam);
  appendIfExists(formData, "LoginTrainer", normalized.loginTrainer);
  appendIfExists(formData, "TypeSport", normalized.typeSport);
  appendIfExists(formData, "LoginOrganization", normalized.loginOrganization);

  if (normalized.photo) {
    formData.append("Photo", normalized.photo);
  }

  const athletesJson =
    normalized.athletsJson ??
    JSON.stringify(
      (normalized.athlets ?? []).map((athlete) => ({
        nameTeam: athlete.nameTeam ?? normalized.nameTeam ?? "",
        loginAthlets: athlete.loginAthlets ?? "",
        athleteStatus: athlete.athleteStatus ?? "Active",
      }))
    );

  formData.append("AthletsJson", athletesJson);

  return formData;
}

function normalizeOrganizationTeam(data: OrganizationTeamDto | any) {
  const loginOrganization =
    data.loginOrganization ??
    data.LoginOrganization ??
    data.organizationLogin ??
    "";

  const nameTeam =
    data.nameTeam ??
    data.NameTeam ??
    data.nameComand ??
    data.NameComand ??
    data.teamName ??
    data.TeamName ??
    "";

  return {
    LoginOrganization: loginOrganization,
    NameTeam: nameTeam,
    loginOrganization,
    nameTeam,
    nameComand: nameTeam,
    teamName: nameTeam,
  };
}

export const commandApi = {
  createTeam(data: TeamModelDto | FormData) {
    return apiRequest("/team/create-team", {
      baseUrl: COMMAND_API_URL,
      method: "POST",
      body: createTeamFormData(data),
    });
  },

  addAthletes(data: TeamAthleteDto[]) {
    return apiRequest("/team/add-athletes", {
      baseUrl: COMMAND_API_URL,
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  linkTeamOrganization(data: OrganizationTeamDto | any) {
    return apiRequest("/team/link-team-organization", {
      baseUrl: COMMAND_API_URL,
      method: "POST",
      body: JSON.stringify(normalizeOrganizationTeam(data)),
    });
  },

  changeStatusAthlet(data: NewSatatusAthlete) {
    return apiRequest("/team/change-status-athlet", {
      baseUrl: COMMAND_API_URL,
      method: "PUT",
      body: JSON.stringify(data),
    });
  },

  removeAthlet(data: NewSatatusAthlete) {
    return apiRequest("/team/remove-athlet", {
      baseUrl: COMMAND_API_URL,
      method: "DELETE",
      body: JSON.stringify(data),
    });
  },

  getAthletesTeam(team: string) {
    return apiRequest(`/athelets-team?team=${encodeURIComponent(team)}`, {
      baseUrl: COMMAND_API_URL,
      method: "GET",
    });
  },

  getTeam(nameTeam: string) {
    return apiRequest<TeamInfoDto>(`/team/${encodeURIComponent(nameTeam)}`, {
      baseUrl: COMMAND_API_URL,
      method: "GET",
    });
  },

  getTeamsByOrganization(loginOrganization: string) {
    return apiRequest<TeamInfoDto[]>(
      `/team/by-organization/${encodeURIComponent(loginOrganization)}`,
      {
        baseUrl: COMMAND_API_URL,
        method: "GET",
      }
    );
  },

  getTeamsByTrainer(loginTrainer: string) {
    return apiRequest<TeamInfoDto[]>(
      `/team/by-trainer/${encodeURIComponent(loginTrainer)}`,
      {
        baseUrl: COMMAND_API_URL,
        method: "GET",
      }
    );
  },

  getPhotoUrl(filePath?: string | null) {
    if (!filePath) return "";
    return `${COMMAND_API_URL}/photo/${encodeURIComponent(filePath)}`;
  },
};
