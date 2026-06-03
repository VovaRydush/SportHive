export type SportRuleDto = {
  sport: string;
  displayName: string;
  matchMode: string;
  liveEvents: string[];
  resultFields: string[];
  scoreExamples: string[];
  fieldLabels: Record<string, string>;
  eventLabels: Record<string, string>;
  scorePatternHint: string;
  allowDraw: boolean;
  maxPeriods: number;
};

export type SportLiveEventDto = {
  id: number;
  type: string;
  participant: string;
  player?: string | null;
  minute?: number | null;
  period?: number | null;
  value?: string | null;
  notes?: string | null;
  createdBy?: string | null;
  createdAt: string;
};

export type SportMatchStateDto = {
  matchType: string;
  matchId: number;
  idEvent: number;
  sport: string;
  status: string;
  firstParticipant: string;
  secondParticipant: string;
  score?: string | null;
  winner?: string | null;
  currentPeriod: number;
  time?: string | null;
  timeline: SportLiveEventDto[];
  stats: Record<string, string>;
  rules: SportRuleDto;
  canEdit: boolean;
  validationMessages: string[];
};

export type SportLiveEventSubmitDto = {
  matchType: string;
  matchId: number;
  type: string;
  participant: string;
  player?: string;
  minute?: number;
  period?: number;
  value?: string;
  notes?: string;
};

export type SportFinalResultSubmitDto = {
  matchType: string;
  matchId: number;
  score: string;
  winner?: string;
  stats: Record<string, string>;
  notes?: string;
  finishMatch: boolean;
};
