import { extractScore, extractWinner, getEffectiveMatchStatus } from "./matchStatus";

export type HomeLeaderType = "team" | "athlete" | "judge";

export type HomeLeaderRow = {
  id: string;
  name: string;
  type: HomeLeaderType;
  sport: string;
  played: number;
  wins: number;
  draws: number;
  losses: number;
  points: number;
  scoreFor: number;
  scoreAgainst: number;
  scoreDiff: number;
};

type AnyObj = Record<string, any>;

function clean(value: unknown) {
  return String(value ?? "").trim();
}

function key(value: unknown) {
  return clean(value).toLowerCase();
}

function isTeamMatch(match: AnyObj) {
  const type = clean(match.matchType || match.MatchType).toLowerCase();
  return type.includes("team") || Boolean(match.nameFirstTeam || match.NameFirstTeam || match.nameSecondTeam || match.NameSecondTeam);
}

function participantFirst(match: AnyObj) {
  return clean(
    match.firstParticipant ||
    match.firstSide ||
    match.nameFirstTeam ||
    match.NameFirstTeam ||
    match.loginFirstAthlete ||
    match.LoginFirstAthlete
  );
}

function participantSecond(match: AnyObj) {
  return clean(
    match.secondParticipant ||
    match.secondSide ||
    match.nameSecondTeam ||
    match.NameSecondTeam ||
    match.loginSecondAthlete ||
    match.LoginSecondAthlete
  );
}

function getRow(map: Map<string, HomeLeaderRow>, name: string, type: HomeLeaderType, sport: string) {
  const id = key(name);

  if (!map.has(id)) {
    map.set(id, {
      id,
      name,
      type,
      sport,
      played: 0,
      wins: 0,
      draws: 0,
      losses: 0,
      points: 0,
      scoreFor: 0,
      scoreAgainst: 0,
      scoreDiff: 0,
    });
  }

  return map.get(id)!;
}

function parseScore(score: string) {
  const cleanScore = clean(score).replace(/\s/g, "").replace("-", ":");
  const parts = cleanScore.split(":").map(Number);

  if (parts.length !== 2 || parts.some(x => !Number.isFinite(x))) {
    return null;
  }

  return { first: parts[0], second: parts[1] };
}

function isWinner(winner: string, name: string) {
  return key(winner) === key(name);
}

function applyResult(first: HomeLeaderRow, second: HomeLeaderRow, match: AnyObj) {
  first.played++;
  second.played++;

  const score = parseScore(extractScore(match) || clean(match.score || match.Score));
  const winner = extractWinner(match) || clean(match.winner || match.Winner);

  if (score) {
    first.scoreFor += score.first;
    first.scoreAgainst += score.second;
    second.scoreFor += score.second;
    second.scoreAgainst += score.first;

    first.scoreDiff = first.scoreFor - first.scoreAgainst;
    second.scoreDiff = second.scoreFor - second.scoreAgainst;

    if (score.first > score.second) {
      first.wins++;
      first.points += 3;
      second.losses++;
    } else if (score.second > score.first) {
      second.wins++;
      second.points += 3;
      first.losses++;
    } else {
      first.draws++;
      second.draws++;
      first.points++;
      second.points++;
    }

    return;
  }

  if (winner) {
    if (isWinner(winner, first.name)) {
      first.wins++;
      first.points += 3;
      second.losses++;
    } else if (isWinner(winner, second.name)) {
      second.wins++;
      second.points += 3;
      first.losses++;
    }
  }
}

function sortLeaders(rows: HomeLeaderRow[]) {
  return rows.sort((a, b) =>
    b.points - a.points ||
    b.wins - a.wins ||
    b.scoreDiff - a.scoreDiff ||
    b.played - a.played ||
    a.name.localeCompare(b.name)
  );
}

export function buildTournamentHomeLeaders(events: AnyObj[]) {
  const teamMap = new Map<string, HomeLeaderRow>();
  const athleteMap = new Map<string, HomeLeaderRow>();
  const judgeMap = new Map<string, HomeLeaderRow>();

  for (const event of events || []) {
    const sport = clean(event.typeSport || event.TypeSport || "Sport");
    const matches = Array.isArray(event.matches) ? event.matches : [];

    for (const match of matches) {
      const status = getEffectiveMatchStatus(match);
      const judge = clean(match.loginJudge || match.LoginJudge || match.judgeLogin);

      if (judge) {
        const row = getRow(judgeMap, judge, "judge", sport);
        row.played++;
        if (status === "Finished") row.points++;
      }

      if (status !== "Finished") continue;

      const firstName = participantFirst(match);
      const secondName = participantSecond(match);

      if (!firstName || !secondName) continue;

      const type: HomeLeaderType = isTeamMatch(match) ? "team" : "athlete";
      const map = type === "team" ? teamMap : athleteMap;

      const first = getRow(map, firstName, type, sport);
      const second = getRow(map, secondName, type, sport);

      applyResult(first, second, match);
    }
  }

  return {
    teams: sortLeaders(Array.from(teamMap.values())).slice(0, 5),
    athletes: sortLeaders(Array.from(athleteMap.values())).slice(0, 5),
    judges: sortLeaders(Array.from(judgeMap.values())).slice(0, 5),
  };
}
