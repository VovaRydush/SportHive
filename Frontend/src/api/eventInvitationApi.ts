import { EVENT_API_URL } from "./http";
import { authHeaders, getCurrentLogin } from "./authToken";

export type EventInviteParticipantType = "team" | "individual";

export type EventInviteParticipant = {
  id: string;
  name: string;
  type: EventInviteParticipantType;
  subtitle?: string;
  email?: string;
  ownerOrganizationLogin?: string;
};

export type CreateEventInvitationRequest = {
  nameEvent: string;
  typeSport: string;
  systems: number | string;
  participantType: EventInviteParticipantType;
  participants: string[];
  dataStart: string;
  dataEnd?: string | null;
  description?: string;
  loginJudge?: string | null;
  createdByOrganization?: string | null;
};

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const response = await fetch(`${EVENT_API_URL}${path}`, {
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

export const eventInvitationApi = {
  searchParticipants(participantType: EventInviteParticipantType, typeSport: string, query: string) {
    return request<EventInviteParticipant[]>(
      `/event-invitations/search-participants?participantType=${encodeURIComponent(participantType)}&typeSport=${encodeURIComponent(typeSport)}&query=${encodeURIComponent(query)}`
    );
  },

  createInvitation(data: CreateEventInvitationRequest) {
    return request<{ batchId: number; deadlineAt: string; totalInvited: number; acceptedCount: number; requiredCount: number; status: string }>(
      "/event-invitations/create",
      {
        method: "POST",
        body: JSON.stringify({
          ...data,
          createdByOrganization: data.createdByOrganization || getCurrentLogin(),
        }),
      }
    );
  },
};
