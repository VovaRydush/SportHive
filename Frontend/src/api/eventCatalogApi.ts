import { apiRequest, EVENT_API_URL } from "./http";
import type {
  EventCatalogItemDto,
  EventCatalogQueryDto,
  MatchResultSubmitDto,
} from "./eventCatalogTypes";

function currentUserQuery() {
  const login = localStorage.getItem("login") || "";
  const role = localStorage.getItem("userRole") || localStorage.getItem("role") || "";

  return { login, role };
}

function toQuery(query: Record<string, string | undefined | null>) {
  const params = new URLSearchParams();

 for (const key in query) {
  const value = query[key];

  if (value !== undefined && value !== null && value !== "") {
    params.set(key, String(value));
  }
}

  const text = params.toString();
  return text ? `?${text}` : "";
}

export const eventCatalogApi = {
  getEvents(query: EventCatalogQueryDto = {}) {
    const user = currentUserQuery();

    return apiRequest<EventCatalogItemDto[]>(
      `/events-catalog${toQuery({ ...query, login: user.login, role: user.role })}`,
      {
        baseUrl: EVENT_API_URL,
        method: "GET",
        auth: false,
      }
    );
  },

  getEvent(idEvent: number) {
    const user = currentUserQuery();

    return apiRequest<EventCatalogItemDto>(
      `/events-catalog/${idEvent}${toQuery(user)}`,
      {
        baseUrl: EVENT_API_URL,
        method: "GET",
        auth: false,
      }
    );
  },

  submitResult(data: Omit<MatchResultSubmitDto, "login" | "role">) {
    const user = currentUserQuery();

    return apiRequest(`/events-catalog/match-result`, {
      baseUrl: EVENT_API_URL,
      method: "POST",
      body: JSON.stringify({
        ...data,
        login: user.login,
        role: user.role,
      }),
    });
  },

  generateNextRound(idEvent: number) {
    const user = currentUserQuery();

    return apiRequest<EventCatalogItemDto>(
      `/events-catalog/${idEvent}/generate-next-round${toQuery(user)}`,
      {
        baseUrl: EVENT_API_URL,
        method: "POST",
      }
    );
  },

  rebuild(idEvent: number) {
    const user = currentUserQuery();

    return apiRequest<EventCatalogItemDto>(
      `/events-catalog/${idEvent}/rebuild${toQuery(user)}`,
      {
        baseUrl: EVENT_API_URL,
        method: "POST",
      }
    );
  },
};
