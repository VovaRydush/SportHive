import { apiRequest, COMMAND_API_URL } from "./http";
import type {
  NewSatatusAthlete,
  OrganizationTeamDto,
  TeamAthleteDto,
  TeamInfoDto,
  TeamModelDto,
} from "./commandTypes";

function appendIfExists(formData: FormData, key: string, value: unknown) {
  if (value !== undefined && value !== null && value !== "") {
    formData.append(key, value as string | Blob);
  }
}

function createTeamFormData(data: TeamModelDto) {
  const formData = new FormData();

  appendIfExists(formData, "nameTeam", data.nameTeam);
  appendIfExists(formData, "loginTrainer", data.loginTrainer);
  appendIfExists(formData, "typeSport", data.typeSport);

  if (data.photo) {
    formData.append("photo", data.photo);
  }

  const athletesJson =
    data.athletsJson ??
    JSON.stringify(
      (data.athlets ?? []).map((athlete) => ({
        nameTeam: athlete.nameTeam ?? data.nameTeam ?? "",
        loginAthlets: athlete.loginAthlets ?? "",
        athleteStatus: athlete.athleteStatus ?? "Active",
      }))
    );

  formData.append("athletsJson", athletesJson);

  return formData;
}

export const commandApi = {
  createTeam(data: TeamModelDto) {
    return apiRequest<any>("/team/create-team", {
      baseUrl: COMMAND_API_URL,
      method: "POST",
      body: createTeamFormData(data),
    });
  },

  addAthletes(data: TeamAthleteDto[]) {
    return apiRequest<any>("/team/add-athletes", {
      baseUrl: COMMAND_API_URL,
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  linkTeamOrganization(data: OrganizationTeamDto) {
    return apiRequest<any>("/team/link-team-organization", {
      baseUrl: COMMAND_API_URL,
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  changeStatusAthlet(data: NewSatatusAthlete) {
    return apiRequest<any>("/team/change-status-athlet", {
      baseUrl: COMMAND_API_URL,
      method: "PUT",
      body: JSON.stringify(data),
    });
  },

  removeAthlet(data: NewSatatusAthlete) {
    return apiRequest<any>("/team/remove-athlet", {
      baseUrl: COMMAND_API_URL,
      method: "DELETE",
      body: JSON.stringify(data),
    });
  },

  getAthletesTeam(team: string) {
    return apiRequest<any>(`/athelets-team?team=${encodeURIComponent(team)}`, {
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
    return apiRequest<TeamInfoDto[]>(`/team/by-organization/${encodeURIComponent(loginOrganization)}`, {
      baseUrl: COMMAND_API_URL,
      method: "GET",
    });
  },

  getTeamsByTrainer(loginTrainer: string) {
    return apiRequest<TeamInfoDto[]>(`/team/by-trainer/${encodeURIComponent(loginTrainer)}`, {
      baseUrl: COMMAND_API_URL,
      method: "GET",
    });
  },

  getPhotoUrl(filePath?: string | null) {
    if (!filePath) return "";
    return `${COMMAND_API_URL}/photo/${encodeURIComponent(filePath)}`;
  },
};
