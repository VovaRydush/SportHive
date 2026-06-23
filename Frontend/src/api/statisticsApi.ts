export type StatisticsFilter = {
  season?: string;
  sport?: string;
  system?: string;
  level?: string;
  sortBy?: string;
  direction?: string;
};

export type LeaderboardRowDto = {
  id: string;
  name: string;
  type: string;
  sport?: string;
  organizationLogin?: string;
  organizationName?: string;
  played: number;
  finished: number;
  wins: number;
  draws: number;
  losses: number;
  scoreFor: number;
  scoreAgainst: number;
  scoreDiff: number;
  points: number;
  winRate: number;
};

export type StatisticsDashboardDto = {
  filter: StatisticsFilter;
  summary: {
    events: number;
    matches: number;
    finishedMatches: number;
    liveMatches: number;
    upcomingMatches: number;
    participants: number;
    teams: number;
    organizations: number;
  };
  seasons: string[];
  sports: string[];
  athleteLeaders: LeaderboardRowDto[];
  teamLeaders: LeaderboardRowDto[];
  organizationLeaders: LeaderboardRowDto[];
  judgeLeaders: LeaderboardRowDto[];
  recentFinishedMatches: unknown[];
};

const EVENT_API_BASE =
  (window as any).SPORT_HIVE_EVENT_API ||
  localStorage.getItem("SPORT_HIVE_EVENT_API") ||
  "http://localhost:5042";

function token() {
  return localStorage.getItem("token") ||
    localStorage.getItem("accessToken") ||
    localStorage.getItem("jwt") ||
    "";
}

async function requestJson<T>(url: string, options: RequestInit = {}): Promise<T> {
  const headers = new Headers(options.headers || {});
  headers.set("Content-Type", "application/json");

  const jwt = token();
  if (jwt) headers.set("Authorization", jwt.startsWith("Bearer ") ? jwt : `Bearer ${jwt}`);

  const res = await fetch(url, { ...options, headers });

  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(text || `HTTP ${res.status}`);
  }

  return await res.json() as T;
}

function qs(filter: StatisticsFilter = {}) {
  const params = new URLSearchParams();

  Object.entries(filter).forEach(([key, value]) => {
    if (value && value !== "all") params.set(key, value);
  });

  const s = params.toString();
  return s ? `?${s}` : "";
}

export const statisticsApi = {
  getGlobal(filter: StatisticsFilter = {}) {
    return requestJson<StatisticsDashboardDto>(`${EVENT_API_BASE}/statistics/global${qs(filter)}`);
  },

  getOrganization(login: string, filter: StatisticsFilter = {}) {
    return requestJson<StatisticsDashboardDto>(`${EVENT_API_BASE}/statistics/organization/${encodeURIComponent(login)}${qs(filter)}`);
  },
};

export const managementApi = {
  updateTeam(teamName: string, body: { newTeamName?: string; typeSport?: string; loginTrainer?: string; teamPhoto?: string }) {
    return requestJson(`${EVENT_API_BASE}/management/teams/${encodeURIComponent(teamName)}`, {
      method: "PUT",
      body: JSON.stringify(body),
    });
  },

  deleteTeam(teamName: string) {
    return requestJson(`${EVENT_API_BASE}/management/teams/${encodeURIComponent(teamName)}`, {
      method: "DELETE",
    });
  },

  addTeamAthlete(teamName: string, athleteLogin: string, athleteStatus = "Active") {
    return requestJson(`${EVENT_API_BASE}/management/teams/${encodeURIComponent(teamName)}/athletes`, {
      method: "POST",
      body: JSON.stringify({ athleteLogin, athleteStatus }),
    });
  },

  removeTeamAthlete(teamName: string, athleteLogin: string) {
    return requestJson(`${EVENT_API_BASE}/management/teams/${encodeURIComponent(teamName)}/athletes/${encodeURIComponent(athleteLogin)}`, {
      method: "DELETE",
    });
  },

  addOrganizationMember(organizationLogin: string, role: string, login: string) {
    return requestJson(`${EVENT_API_BASE}/management/organizations/${encodeURIComponent(organizationLogin)}/members`, {
      method: "POST",
      body: JSON.stringify({ role, login }),
    });
  },

  removeOrganizationMember(organizationLogin: string, role: string, login: string) {
    return requestJson(`${EVENT_API_BASE}/management/organizations/${encodeURIComponent(organizationLogin)}/members/${encodeURIComponent(role)}/${encodeURIComponent(login)}`, {
      method: "DELETE",
    });
  },
};
