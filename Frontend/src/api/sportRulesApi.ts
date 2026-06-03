import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import { apiRequest, EVENT_API_URL } from "./http";
import type {
  SportFinalResultSubmitDto,
  SportLiveEventSubmitDto,
  SportMatchStateDto,
  SportRuleDto,
} from "./sportRulesTypes";

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
      params.set(key, value);
    }
  }

  const text = params.toString();
  return text ? `?${text}` : "";
}

export const sportRulesApi = {
  getRules() {
    return apiRequest<SportRuleDto[]>("/sport-rules", {
      baseUrl: EVENT_API_URL,
      method: "GET",
      auth: false,
    });
  },

  getMatchState(matchType: string, matchId: number) {
    return apiRequest<SportMatchStateDto>(
      `/sport-rules/match/${encodeURIComponent(matchType)}/${matchId}${toQuery(currentUserQuery())}`,
      {
        baseUrl: EVENT_API_URL,
        method: "GET",
        auth: false,
      }
    );
  },

  addLiveEvent(data: SportLiveEventSubmitDto) {
    const user = currentUserQuery();

    return apiRequest<SportMatchStateDto>("/sport-rules/match/live-event", {
      baseUrl: EVENT_API_URL,
      method: "POST",
      body: JSON.stringify({
        ...data,
        login: user.login,
        role: user.role,
      }),
    });
  },

  submitFinalResult(data: SportFinalResultSubmitDto) {
    const user = currentUserQuery();

    return apiRequest<SportMatchStateDto>("/sport-rules/match/final-result", {
      baseUrl: EVENT_API_URL,
      method: "POST",
      body: JSON.stringify({
        ...data,
        login: user.login,
        role: user.role,
      }),
    });
  },

  createConnection(matchType: string, matchId: number, onUpdate: (state: SportMatchStateDto) => void): HubConnection {
    const connection = new HubConnectionBuilder()
      .withUrl(`${EVENT_API_URL}/hubs/match-live`)
      .withAutomaticReconnect()
      .build();

    connection.on("MatchUpdated", onUpdate);

    connection.start()
      .then(() => connection.invoke("JoinMatch", matchType, matchId))
      .catch((error) => console.error("SignalR connection error", error));

    return connection;
  },
};
