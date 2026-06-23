import { AUTH_API_URL } from "./http";
import { authHeaders, getCurrentLogin, getCurrentRole, type UserRole } from "./authToken";

export type ProfileTeam = {
  teamName: string;
  typeSport?: string;
  loginTrainer?: string;
  athletesCount?: number;
  photoTeam?: string;
};

export type ProfileOrganization = {
  loginOrganization: string;
  nameOrganization: string;
  country?: string;
  typeOrganization?: string;
  profilePhoto?: string;
};

export type ProfileMatch = {
  id?: number;
  idEvent?: number;
  nameEvent?: string;
  type?: string;
  firstSide?: string;
  secondSide?: string;
  score?: string;
  statusMatch?: number;
  tour?: number;
  group?: number;
  dataMatch?: string | null;
  timeMatch?: string | null;
  roleInMatch?: string;
};

export type UserProfileData = {
  login: string;
  role: UserRole;
  fullName: string;
  firstName?: string;
  lastName?: string;
  dataBirth?: string | null;
  age?: number | null;
  mail?: string;
  typeSport?: string | null;
  profilePhoto?: string | null;
  organizations: ProfileOrganization[];
  teams: ProfileTeam[];
  matches: ProfileMatch[];
  judgedMatches: ProfileMatch[];
  upcomingMatches: ProfileMatch[];
  finishedMatches: ProfileMatch[];
  stats: {
    totalMatches: number;
    upcomingMatches: number;
    finishedMatches: number;
    wins: number;
    losses: number;
    draws: number;
    teamsCount: number;
    organizationsCount: number;
  };
};

function roleFrom(value?: string | null): UserRole {
  const text = String(value || "").toLowerCase();

  if (text.includes("organization") || text.includes("орган")) return "Organization";
  if (text.includes("trainer") || text.includes("трен")) return "Trainer";
  if (text.includes("judge") || text.includes("суд")) return "Judge";
  if (text.includes("athlete") || text.includes("спорт")) return "Athlete";

  return "Athlete";
}

function array<T>(value: unknown): T[] {
  return Array.isArray(value) ? (value as T[]) : [];
}

function text(value: unknown): string {
  return value === null || value === undefined ? "" : String(value);
}

function number(value: unknown): number {
  const result = Number(value);
  return Number.isFinite(result) ? result : 0;
}

async function request<T>(path: string): Promise<T> {
  const response = await fetch(`${AUTH_API_URL}${path}`, {
    method: "GET",
    headers: {
      Accept: "application/json",
      ...authHeaders(),
    },
  });

  if (!response.ok) {
    const body = await response.text();
    throw new Error(body || `HTTP error ${response.status}`);
  }

  return (await response.json()) as T;
}

export function normalizeProfile(raw: any, fallbackLogin: string, fallbackRole: UserRole): UserProfileData {
  const role = roleFrom(raw?.role || fallbackRole);

  const teams = array<any>(raw?.teams).map(item => ({
    teamName: text(item.teamName ?? item.TeamName ?? item.nameTeam ?? item.NameTeam),
    typeSport: text(item.typeSport ?? item.TypeSport),
    loginTrainer: text(item.loginTrainer ?? item.LoginTrainer),
    athletesCount: number(item.athletesCount ?? item.AthletesCount),
    photoTeam: text(item.photoTeam ?? item.PhotoTeam ?? item.teamPhoto ?? item.TeamPhoto),
  })).filter(item => item.teamName);

  const organizations = array<any>(raw?.organizations).map(item => ({
    loginOrganization: text(item.loginOrganization ?? item.LoginOrganization ?? item.login ?? item.Login),
    nameOrganization: text(item.nameOrganization ?? item.NameOrganization ?? item.name ?? item.Name),
    country: text(item.country ?? item.Country),
    typeOrganization: text(item.typeOrganization ?? item.TypeOrganization ?? item.typeOrganozation ?? item.TypeOrganozation),
    profilePhoto: text(item.profilePhoto ?? item.ProfilePhoto),
  })).filter(item => item.loginOrganization || item.nameOrganization);

  const normalizeMatch = (item: any): ProfileMatch => ({
    id: number(item.id ?? item.Id),
    idEvent: number(item.idEvent ?? item.IdEvent),
    nameEvent: text(item.nameEvent ?? item.NameEvent),
    type: text(item.type ?? item.Type),
    firstSide: text(item.firstSide ?? item.FirstSide ?? item.firstTeam ?? item.NameFirstTeam ?? item.loginFirstAthlete),
    secondSide: text(item.secondSide ?? item.SecondSide ?? item.secondTeam ?? item.NameSecondTeam ?? item.loginSecondAthlete),
    score: text(item.score ?? item.Score ?? item.addInformation ?? item.AddInformation),
    statusMatch: number(item.statusMatch ?? item.StatusMatch),
    tour: number(item.tour ?? item.Tour),
    group: number(item.group ?? item.Group),
    dataMatch: item.dataMatch ?? item.DataMatch ?? null,
    timeMatch: item.timeMatch ?? item.TimeMatch ?? null,
    roleInMatch: text(item.roleInMatch ?? item.RoleInMatch),
  });

  const matches = array<any>(raw?.matches).map(normalizeMatch);
  const judgedMatches = array<any>(raw?.judgedMatches).map(normalizeMatch);
  const upcomingMatches = array<any>(raw?.upcomingMatches).map(normalizeMatch);
  const finishedMatches = array<any>(raw?.finishedMatches).map(normalizeMatch);

  return {
    login: text(raw?.login ?? raw?.Login) || fallbackLogin,
    role,
    firstName: text(raw?.firstName ?? raw?.FirstName),
    lastName: text(raw?.lastName ?? raw?.LastName),
    fullName: text(raw?.fullName ?? raw?.FullName) || text(raw?.name ?? raw?.Name) || fallbackLogin,
    dataBirth: raw?.dataBirth ?? raw?.DataBirth ?? null,
    age: raw?.age ?? raw?.Age ?? null,
    mail: text(raw?.mail ?? raw?.Mail ?? raw?.email ?? raw?.Email),
    typeSport: raw?.typeSport ?? raw?.TypeSport ?? null,
    profilePhoto: raw?.profilePhoto ?? raw?.ProfilePhoto ?? null,
    organizations,
    teams,
    matches,
    judgedMatches,
    upcomingMatches,
    finishedMatches,
    stats: {
      totalMatches: number(raw?.stats?.totalMatches ?? raw?.stats?.TotalMatches ?? matches.length + judgedMatches.length),
      upcomingMatches: number(raw?.stats?.upcomingMatches ?? raw?.stats?.UpcomingMatches ?? upcomingMatches.length),
      finishedMatches: number(raw?.stats?.finishedMatches ?? raw?.stats?.FinishedMatches ?? finishedMatches.length),
      wins: number(raw?.stats?.wins ?? raw?.stats?.Wins),
      losses: number(raw?.stats?.losses ?? raw?.stats?.Losses),
      draws: number(raw?.stats?.draws ?? raw?.stats?.Draws),
      teamsCount: number(raw?.stats?.teamsCount ?? raw?.stats?.TeamsCount ?? teams.length),
      organizationsCount: number(raw?.stats?.organizationsCount ?? raw?.stats?.OrganizationsCount ?? organizations.length),
    },
  };
}

export const userProfileApi = {
  async get(login: string, role: UserRole): Promise<UserProfileData> {
    const raw = await request<any>(
      `/profiles/member?login=${encodeURIComponent(login)}&role=${encodeURIComponent(role)}`
    );

    return normalizeProfile(raw, login, role);
  },

  async getMe(): Promise<UserProfileData> {
    const login = getCurrentLogin();
    const role = getCurrentRole();

    if (!login) {
      throw new Error("Не знайдено login поточного користувача. Перелогінься.");
    }

    return this.get(login, role);
  },
};
