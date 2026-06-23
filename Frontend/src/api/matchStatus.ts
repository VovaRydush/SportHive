export type EffectiveMatchStatus = "Upcoming" | "Live" | "Finished";

type AnyMatch = Record<string, any>;

function pick(match: AnyMatch, ...keys: string[]) {
  for (const key of keys) {
    if (match && match[key] !== undefined && match[key] !== null) return match[key];
  }

  return undefined;
}

export function parseAddInformation(value?: string | null): Record<string, string> {
  const result: Record<string, string> = {};
  const raw = String(value || "").trim();

  if (!raw) return result;

  raw.split(";").forEach(part => {
    const index = part.indexOf("=");

    if (index <= 0) return;

    const key = part.slice(0, index).trim();
    const itemValue = part.slice(index + 1).trim();

    if (key) result[key] = itemValue;
  });

  return result;
}

export function extractScore(match: AnyMatch): string {
  const rawScore = String(pick(match, "score", "Score") || "").trim();
  const addInformation = String(pick(match, "addInformation", "AddInformation") || "").trim();
  const parsed = parseAddInformation(addInformation || rawScore);

  if (parsed.score && /^\d+\s*[:\-]\s*\d+$/.test(parsed.score)) {
    return parsed.score.replace(/\s/g, "").replace("-", ":");
  }

  if (/^\d+\s*[:\-]\s*\d+$/.test(rawScore)) {
    return rawScore.replace(/\s/g, "").replace("-", ":");
  }

  if (/^\d+\s*[:\-]\s*\d+$/.test(addInformation)) {
    return addInformation.replace(/\s/g, "").replace("-", ":");
  }

  const scoreFromText = (addInformation || rawScore).match(/score=(\d+\s*[:\-]\s*\d+)/i);

  if (scoreFromText?.[1]) {
    return scoreFromText[1].replace(/\s/g, "").replace("-", ":");
  }

  return "";
}

export function extractWinner(match: AnyMatch): string {
  const raw = String(pick(match, "winner", "Winner") || "").trim();

  if (raw) return raw;

  const addInformation = String(pick(match, "addInformation", "AddInformation", "score", "Score") || "").trim();
  const parsed = parseAddInformation(addInformation);

  return parsed.winner || "";
}

export function isFinishedByData(match: AnyMatch): boolean {
  const status = String(pick(match, "status", "Status") || "").toLowerCase();
  const statusMatch = Number(pick(match, "statusMatch", "StatusMatch") ?? -1);

  if (status === "finished" || status === "ft" || statusMatch === 2) return true;

  const raw = String(pick(match, "addInformation", "AddInformation", "score", "Score") || "").trim();
  const parsed = parseAddInformation(raw);

  if (parsed.finishedAt || parsed.winner) return true;
  if (/finishedAt=/i.test(raw)) return true;
  if (/winner=[^;]+/i.test(raw)) return true;

  return false;
}

export function getMatchDate(match: AnyMatch): Date | null {
  const rawDate = pick(match, "dataMatch", "DataMatch", "dateMatch", "DateMatch");
  const rawTime = pick(match, "timeMatch", "TimeMatch");

  if (!rawDate) return null;

  const dateText = String(rawDate);
  const datePart = dateText.includes("T") ? dateText.split("T")[0] : dateText.split(" ")[0];

  let timePart = String(rawTime || "").trim();

  if (!timePart || timePart === "null" || timePart === "undefined") {
    const fallback = new Date(dateText);
    return Number.isNaN(fallback.getTime()) ? null : fallback;
  }

  if (timePart.includes(".")) {
    const parts = timePart.split(".");
    timePart = parts.length >= 3 ? parts[parts.length - 1] : parts[0];
  }

  if (timePart.length === 5) timePart += ":00";

  const local = new Date(`${datePart}T${timePart}`);

  if (!Number.isNaN(local.getTime())) return local;

  const fallback = new Date(dateText);

  return Number.isNaN(fallback.getTime()) ? null : fallback;
}

export function getEffectiveMatchStatus(match: AnyMatch, now: Date = new Date()): EffectiveMatchStatus {
  if (isFinishedByData(match)) return "Finished";

  const date = getMatchDate(match);

  if (date && date.getTime() > now.getTime()) return "Upcoming";
  if (date && date.getTime() <= now.getTime()) return "Live";

  const status = String(pick(match, "status", "Status") || "").toLowerCase();
  const statusMatch = Number(pick(match, "statusMatch", "StatusMatch") ?? -1);

  if (status === "live" || statusMatch === 1) return "Live";

  return "Upcoming";
}

export function canEnterMatchData(match: AnyMatch): boolean {
  if (!pick(match, "canEdit", "CanEdit")) return false;
  if (getEffectiveMatchStatus(match) === "Finished") return false;

  const date = getMatchDate(match);

  if (!date) return false;

  return date.getTime() <= Date.now();
}

export function statusLabel(status: EffectiveMatchStatus | string): string {
  if (status === "Finished") return "FT";
  if (status === "Live") return "LIVE";
  return "NS";
}

export function statusUa(status: EffectiveMatchStatus | string): string {
  if (status === "Finished") return "Завершено";
  if (status === "Live") return "Live";
  return "Очікується";
}

export function readinessLabel(match: AnyMatch): string {
  const status = getEffectiveMatchStatus(match);

  if (status === "Finished") return "Матч завершений. Доступний перегляд результату.";
  if (status === "Live") return "Час матчу настав. Суддя може вносити live-дані та результат.";

  return "Матч ще очікується. Доступна тільки загальна інформація.";
}

export function formatMatchTime(match: AnyMatch): string {
  const status = getEffectiveMatchStatus(match);

  if (status === "Finished") return "FT";
  if (status === "Live") return "LIVE";

  const date = getMatchDate(match);

  if (!date) return `R${pick(match, "tour", "Tour") ?? "-"}`;

  return date.toLocaleTimeString("uk-UA", { hour: "2-digit", minute: "2-digit" });
}

export function formatMatchDateTime(match: AnyMatch): string {
  const date = getMatchDate(match);

  if (!date) return "Дата/час не вказані";

  return date.toLocaleString("uk-UA", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}
