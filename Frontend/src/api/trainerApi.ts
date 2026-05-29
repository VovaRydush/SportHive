import { apiRequest, COMMAND_API_URL } from "./http";
import type { TrainerProfileDto, TrainerTeamDto } from "./trainerTypes";

export const trainerApi = {
  getTrainerProfile(login: string) {
    return apiRequest<TrainerProfileDto>(`/trainer/${encodeURIComponent(login)}`, {
      baseUrl: COMMAND_API_URL,
      method: "GET",
    });
  },

  getTrainerTeams(login: string) {
    return apiRequest<TrainerTeamDto[]>(`/trainer/${encodeURIComponent(login)}/teams`, {
      baseUrl: COMMAND_API_URL,
      method: "GET",
    });
  },
};
