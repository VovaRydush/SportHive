import {
  extractScore,
  extractWinner,
  getEffectiveMatchStatus,
  type EffectiveMatchStatus,
} from "./matchStatus";

export type AnyMatch = Record<string, any>;
export type AnyEvent = Record<string, any>;

export type UiStandingRow = {
  participant: string;
  participantLogin?: string | null;
  group?: number | null;
  played: number;
  wins: number;
  draws: number;
  losses: number;
  scoreFor: number;
  scoreAgainst: number;
  scoreDiff: number;
  points: number;
};

export type UiEventStats = {
  totalMatches: number;
  finishedMatches: number;
  liveMatches: number;
  upcomingMatches: number;
  status: EffectiveMatchStatus;
};

function num(value: unknown) {
  const n = Number(value);
  return Number.isFinite(n) ? n : 0;
}

function key(name: string, login?: string | null) {
  return String(login || name || "").trim().toLowerCase();
}

function name(value: unknown) {
  return String(value || "").trim() || "-";
}

function row(map: Map<string, UiStandingRow>, participant: string, login?: string | null, group?: number | null) {
  const k = key(participant, login);

  if (!map.has(k)) {
    map.set(k, {
      participant: name(participant),
      participantLogin: login || null,
      group: group ?? null,
      played: 0,
      wins: 0,
      draws: 0,
      losses: 0,
      scoreFor: 0,
      scoreAgainst: 0,
      scoreDiff: 0,
      points: 0,
    });
  }

  return map.get(k)!;
}

function parseScore(score: string) {
  const clean = String(score || "").trim().replace(/\s/g, "").replace("-", ":");
  const parts = clean.split(":").map(Number);

  if (parts.length !== 2 || parts.some(x => !Number.isFinite(x))) return null;

  return { first: parts[0], second: parts[1] };
}

function winnerIs(winner: string, participant: string, login?: string | null) {
  const w = winner.trim().toLowerCase();
  return w === String(participant || "").trim().toLowerCase() || w === String(login || "").trim().toLowerCase();
}

export function normalizeMatchForUi(match: AnyMatch): AnyMatch {
  const status = getEffectiveMatchStatus(match);
  const score = extractScore(match) || String(match.score || match.Score || "");
  const winner = extractWinner(match) || String(match.winner || match.Winner || "");

  return { ...match, status, score, winner };
}

export function getEventMatches(event: AnyEvent): AnyMatch[] {
  const matches = Array.isArray(event?.matches) ? event.matches : [];
  return matches.map(normalizeMatchForUi);
}

export function calculateEventStats(event: AnyEvent): UiEventStats {
  const matches = getEventMatches(event);
  const totalMatches = matches.length;
  const finishedMatches = matches.filter(m => getEffectiveMatchStatus(m) === "Finished").length;
  const liveMatches = matches.filter(m => getEffectiveMatchStatus(m) === "Live").length;
  const upcomingMatches = matches.filter(m => getEffectiveMatchStatus(m) === "Upcoming").length;

  let status: EffectiveMatchStatus = "Upcoming";
  if (totalMatches > 0 && finishedMatches === totalMatches) status = "Finished";
  else if (liveMatches > 0) status = "Live";

  return { totalMatches, finishedMatches, liveMatches, upcomingMatches, status };
}

export function buildStandingsFromMatches(matchesInput: AnyMatch[]): UiStandingRow[] {
  const map = new Map<string, UiStandingRow>();
  const matches = (matchesInput || []).map(normalizeMatchForUi);

  for (const match of matches) {
    if (getEffectiveMatchStatus(match) !== "Finished") continue;

    const first = name(match.firstParticipant || match.firstSide || match.NameFirstTeam || match.loginFirstAthlete);
    const second = name(match.secondParticipant || match.secondSide || match.NameSecondTeam || match.loginSecondAthlete);

    if (first === "-" || second === "-") continue;

    const firstLogin = match.firstLogin || match.loginFirstAthlete || null;
    const secondLogin = match.secondLogin || match.loginSecondAthlete || null;
    const group = match.group ?? match.Group ?? null;

    const a = row(map, first, firstLogin, group);
    const b = row(map, second, secondLogin, group);

    a.played++;
    b.played++;

    const score = parseScore(extractScore(match) || match.score || "");
    const winner = extractWinner(match) || "";

    if (score) {
      a.scoreFor += score.first;
      a.scoreAgainst += score.second;
      b.scoreFor += score.second;
      b.scoreAgainst += score.first;
      a.scoreDiff = a.scoreFor - a.scoreAgainst;
      b.scoreDiff = b.scoreFor - b.scoreAgainst;

      if (score.first > score.second) {
        a.wins++;
        a.points += 3;
        b.losses++;
      } else if (score.second > score.first) {
        b.wins++;
        b.points += 3;
        a.losses++;
      } else {
        a.draws++;
        b.draws++;
        a.points++;
        b.points++;
      }
      continue;
    }

    if (winner) {
      if (winnerIs(winner, first, firstLogin)) {
        a.wins++;
        a.points += 3;
        b.losses++;
      } else if (winnerIs(winner, second, secondLogin)) {
        b.wins++;
        b.points += 3;
        a.losses++;
      }
    }
  }

  return Array.from(map.values()).sort((a, b) =>
    num(a.group) - num(b.group) ||
    b.points - a.points ||
    b.scoreDiff - a.scoreDiff ||
    b.scoreFor - a.scoreFor ||
    a.participant.localeCompare(b.participant)
  );
}

export function normalizeEventForUi(event: AnyEvent): AnyEvent {
  const matches = getEventMatches(event);
  const stats = calculateEventStats({ ...event, matches });
  const computedStandings = buildStandingsFromMatches(matches);
  const backendStandings = Array.isArray(event?.standings) ? event.standings : [];

  return {
    ...event,
    matches,
    status: stats.status,
    totalMatches: stats.totalMatches,
    finishedMatches: stats.finishedMatches,
    liveMatches: stats.liveMatches,
    upcomingMatches: stats.upcomingMatches,
    standings: computedStandings.length ? computedStandings : backendStandings,
    computedStandings,
    bracket: Array.isArray(event?.bracket)
      ? event.bracket.map((round: AnyEvent) => ({
          ...round,
          matches: Array.isArray(round.matches) ? round.matches.map(normalizeMatchForUi) : [],
        }))
      : [],
  };
}

export function formatFinishedCounter(event: AnyEvent) {
  const stats = calculateEventStats(event);
  return `${stats.finishedMatches}/${stats.totalMatches}`;
}
