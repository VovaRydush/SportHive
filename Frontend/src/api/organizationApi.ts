import { AUTH_API_URL } from "./http";
import { authHeaders, getCurrentLogin } from "./authToken";

export type OrgRole = "Athlete" | "Judge" | "Trainer";

export type OrgSearchUser = {
  login: string;
  fullName: string;
  mail: string;
  role: OrgRole;
  typeSport?: string;
  profilePhoto?: string;
};

export type OrgInvite = {
  id: number;
  loginOrganization: string;
  targetLogin: string;
  targetEmail: string;
  targetRole: OrgRole;
  status: "Pending" | "Accepted" | "Declined";
  createdAt: string;
  respondedAt?: string | null;
};

export type OrgMember = {
  login: string;
  fullName: string;
  mail?: string;
  typeSport?: string;
  profilePhoto?: string;
};

export type OrgTeam = {
  teamName: string;
  typeSport: string;
  loginTrainer: string;
  teamPhoto?: string;
  athletesCount: number;
};

export type OrgEvent = {
  idEvent: number;
  nameEvent: string;
  typeSport: string;
  systems: string;
  dataStart: string;
  dataEnd?: string | null;
  finishedMatches: number;
  totalMatches: number;
};

export type OrganizationProfileData = {
  organization: {
    login: string;
    nameOrganization: string;
    typeOrganization: string;
    description: string;
    country: string;
    dateFoundation: string;
    profilePhoto?: string;
  };
  judges: OrgMember[];
  trainers: OrgMember[];
  athletesBySport: Record<string, OrgMember[]>;
  teams: OrgTeam[];
  invitations: OrgInvite[];
  recentEvents: OrgEvent[];
};

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const response = await fetch(`${AUTH_API_URL}${path}`, {
    ...options,
    headers: {
      Accept: "application/json",
      ...(options.body instanceof FormData ? {} : { "Content-Type": "application/json" }),
      ...authHeaders(),
      ...(options.headers || {}),
    },
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP error ${response.status}`);
  }

  return (await response.json()) as T;
}

export const organizationApi = {
  getLogin() {
    return getCurrentLogin();
  },

  getProfile(loginOrganization = getCurrentLogin()) {
    return request<OrganizationProfileData>(
      `/organization/profile-data?loginOrganization=${encodeURIComponent(loginOrganization)}`
    );
  },

  searchUsers(role: OrgRole, query: string, loginOrganization = getCurrentLogin()) {
    return request<OrgSearchUser[]>(
      `/organization/search-users?loginOrganization=${encodeURIComponent(loginOrganization)}&role=${encodeURIComponent(role)}&query=${encodeURIComponent(query)}`
    );
  },

  sendInvitation(targetLogin: string, targetRole: OrgRole, loginOrganization = getCurrentLogin()) {
    return request<OrgInvite>("/organization/invitations", {
      method: "POST",
      body: JSON.stringify({ loginOrganization, targetLogin, targetRole }),
    });
  },

  acceptInvitation(token: string) {
    return request<{ status: string }>("/organization/invitations/accept", {
      method: "POST",
      body: JSON.stringify({ token }),
    });
  },

  declineInvitation(token: string) {
    return request<{ status: string }>("/organization/invitations/decline", {
      method: "POST",
      body: JSON.stringify({ token }),
    });
  },
};
