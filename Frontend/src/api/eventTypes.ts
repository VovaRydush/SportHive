export type SelectionSystem =
  | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10;

export type Stage3CreateEventDto = {
  nameEvent: string;
  systems: SelectionSystem;
  typeSport: string;
  participantType: "team" | "individual" | "extreme";
  participants: string[];
  dataStart: string;
  dataEnd?: string | null;
  loginJudge?: string | null;
  description?: string | null;
  generateMatches: boolean;
};

export type Stage3SetMatchResultDto = {
  matchType: string;
  idMatch: number;
  scoreEntity1: number;
  scoreEntity2: number;
  winner?: string | null;
  isDraw: boolean;
  closeMatch: boolean;
  comment?: string | null;
};

export type Stage3ChangeMatchStatusDto = {
  matchType: string;
  idMatch: number;
  status: "Live" | "Finished" | "Upcoming" | 0 | 1 | 2;
};

export type Stage3MatchDto = {
  idMatch: number;
  idEvent: number;
  matchType: string;
  entity1: string;
  entity2: string;
  tour: number;
  group?: number | null;
  status: string;
  dataMatch?: string | null;
  timeMatch?: string | null;
  locationName?: string | null;
  loginJudge?: string | null;
  addInformation?: string | null;
  score1: number;
  score2: number;
  winner?: string | null;
  played: boolean;
  canEnterResult: boolean;
};

export type Stage3StandingDto = {
  name: string;
  played: number;
  wins: number;
  draws: number;
  losses: number;
  scoreFor: number;
  scoreAgainst: number;
  scoreDiff: number;
  points: number;
};

export type Stage3EventWorkspaceDto = {
  idEvent: number;
  nameEvent: string;
  typeSport: string;
  system: number;
  systemName: string;
  dataStart: string;
  dataEnd?: string | null;
  description: string;
  matches: Stage3MatchDto[];
  standings: Stage3StandingDto[];
  summary: {
    participantsCount: number;
    matchesCount: number;
    finishedMatches: number;
    liveMatches: number;
    upcomingMatches: number;
    winner?: string | null;
  };
};
