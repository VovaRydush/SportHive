import { AUTH_API_URL, COMMAND_API_URL, EVENT_API_URL } from "./http";

export type MediaOwner = "auth" | "command" | "event";

const baseByOwner: Record<MediaOwner, string> = {
  auth: AUTH_API_URL,
  command: COMMAND_API_URL,
  event: EVENT_API_URL,
};

export function isProbablyBase64(value: string): boolean {
  const text = value.trim();

  if (text.length < 80) return false;
  if (text.includes("/") || text.includes("\\") || text.includes(".")) return false;

  return /^[A-Za-z0-9+/=_-]+$/.test(text);
}

export function resolvePhotoUrl(value?: string | null, owner: MediaOwner = "auth"): string {
  const raw = String(value || "").trim();

  if (!raw) return "";

  if (
    raw.startsWith("http://") ||
    raw.startsWith("https://") ||
    raw.startsWith("data:") ||
    raw.startsWith("blob:")
  ) {
    return raw;
  }

  if (isProbablyBase64(raw)) {
    return `data:image/jpeg;base64,${raw}`;
  }

  const normalized = raw.replace(/\\/g, "/");

  if (normalized.startsWith("/")) {
    return `${baseByOwner[owner]}${normalized}`;
  }

  return `${baseByOwner[owner]}/photo?path=${encodeURIComponent(normalized)}`;
}

export function initials(value?: string | null): string {
  const text = String(value || "").trim();

  if (!text) return "?";

  const parts = text.split(/\s+/).filter(Boolean);

  if (parts.length >= 2) {
    return `${parts[0][0]}${parts[1][0]}`.toUpperCase();
  }

  return text[0].toUpperCase();
}
