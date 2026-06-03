export type EventMatchCatalogDto = {
  matchType: "team" | "individual" | "extreme";
  matchId: number;
  idEvent: number;
  firstParticipant: string;
  secondParticipant: string;
  firstLogin?: string | null;
  secondLogin?: string | null;
  dataMatch?: string | null;
  timeMatch?: string | null;
  tour: number;
  group?: number | null;
  locationName?: string | null;
  status: "Live" | "Finished" | "Upcoming" | string;
  loginJudge?: string | null;
  addInformation?: string | null;
  score?: string | null;
  winner?: string | null;
  bracketCode?: string | null;
  canEdit: boolean;
  accessReason: string;
};

export type EventStandingDto = {
  participant: string;
  participantLogin?: string | null;
  played: number;
  wins: number;
  draws: number;
  losses: number;
  points: number;
  scoreFor: number;
  scoreAgainst: number;
  scoreDiff: number;
  group: number;
};

export type BracketRoundDto = {
  tour: number;
  group?: number | null;
  bracketCode: string;
  matches: EventMatchCatalogDto[];
};

export type EventCatalogItemDto = {
  idEvent: number;
  nameEvent: string;
  system: string;
  typeSport: string;
  dataStart: string;
  dataEnd?: string | null;
  description: string;
  eventPhoto: string;
  status: string;
  totalMatches: number;
  liveMatches: number;
  finishedMatches: number;
  upcomingMatches: number;
  canManageEvent: boolean;
  accessLevel: string;
  matches: EventMatchCatalogDto[];
  standings: EventStandingDto[];
  bracket: BracketRoundDto[];
};

export type EventCatalogQueryDto = {
  search?: string;
  typeSport?: string;
  system?: string;
  status?: string;
  login?: string;
  role?: string;
};

export type MatchResultSubmitDto = {
  matchType: string;
  matchId: number;
  login?: string | null;
  role?: string | null;
  score: string;
  winner?: string | null;
  notes?: string | null;
  finishMatch: boolean;
};
