import { AUTH_API_URL } from "./http";
import { authHeaders, getCurrentLogin } from "./authToken";

export type UserRole = "Athlete" | "Trainer" | "Judge";

export type UserProfileData = {
  login: string;
  role: UserRole;
  fullName: string;
  firstName?: string;
  lastName?: string;
  dataBirth?: string;
  age?: number;
  mail?: string;
  typeSport?: string;
  profilePhoto?: string;
  organizations: Array<{
    loginOrganization: string;
    nameOrganization: string;
    country?: string;
  }>;
  teams: Array<{
    teamName: string;
    typeSport?: string;
    loginTrainer?: string;
    athletesCount?: number;
  }>;
  matches: Array<{
    id?: number;
    idEvent?: number;
    nameEvent?: string;
    type?: string;
    opponent?: string;
    firstSide?: string;
    secondSide?: string;
    score?: string;
    statusMatch?: number;
    tour?: number;
    group?: number;
    dataMatch?: string;
  }>;
  stats: {
    totalMatches: number;
    finishedMatches: number;
    wins: number;
    losses: number;
    draws: number;
  };
};

async function request<T>(path: string): Promise<T> {
  const response = await fetch(`${AUTH_API_URL}${path}`, {
    method: "GET",
    headers: {
      Accept: "application/json",
      ...authHeaders(),
    },
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP error ${response.status}`);
  }

  return (await response.json()) as T;
}

export const userProfileApi = {
  get(login: string, role: UserRole, loginOrganization = getCurrentLogin()) {
    return request<UserProfileData>(
      `/profiles/member?login=${encodeURIComponent(login)}&role=${encodeURIComponent(role)}&loginOrganization=${encodeURIComponent(loginOrganization)}`
    );
  },
};
