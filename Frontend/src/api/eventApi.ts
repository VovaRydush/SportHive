import { apiRequest, EVENT_API_URL } from "./http";
import type {
  Stage3ChangeMatchStatusDto,
  Stage3CreateEventDto,
  Stage3EventWorkspaceDto,
  Stage3MatchDto,
  Stage3SetMatchResultDto,
} from "./eventTypes";

export const eventApi = {
  createEvent(data: Stage3CreateEventDto) {
    return apiRequest<Stage3EventWorkspaceDto>("/stage3/events", {
      baseUrl: EVENT_API_URL,
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  getWorkspace(eventName: string) {
    return apiRequest<Stage3EventWorkspaceDto>(`/stage3/events/${encodeURIComponent(eventName)}`, {
      baseUrl: EVENT_API_URL,
      method: "GET",
    });
  },

  getMatch(matchType: string, idMatch: number) {
    return apiRequest<Stage3MatchDto>(`/stage3/matches/${encodeURIComponent(matchType)}/${idMatch}`, {
      baseUrl: EVENT_API_URL,
      method: "GET",
    });
  },

  setResult(data: Stage3SetMatchResultDto) {
    return apiRequest<Stage3EventWorkspaceDto>("/stage3/matches/result", {
      baseUrl: EVENT_API_URL,
      method: "PATCH",
      body: JSON.stringify(data),
    });
  },

  changeStatus(data: Stage3ChangeMatchStatusDto) {
    return apiRequest<Stage3MatchDto>("/stage3/matches/status", {
      baseUrl: EVENT_API_URL,
      method: "PATCH",
      body: JSON.stringify(data),
    });
  },
};
