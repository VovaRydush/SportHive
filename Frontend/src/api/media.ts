import { AUTH_API_URL, COMMAND_API_URL, EVENT_API_URL } from "./http";

export type MediaOwner = "auth" | "command" | "event";

const baseByOwner: Record<MediaOwner, string> = {
  auth: AUTH_API_URL,
  command: COMMAND_API_URL,
  event: EVENT_API_URL,
};

export function isProbablyBase64(value: string): boolean {
  const text = String(value || "").trim();

  if (text.length < 80) return false;
  if (text.includes("/") || text.includes("\\") || text.includes(".")) return false;

  return /^[A-Za-z0-9+/=_-]+$/.test(text);
}

export function resolvePhotoUrl(value?: string | null, owner: MediaOwner = "auth"): string {
  const raw = String(value || "").trim();

  if (!raw || raw === "null" || raw === "undefined") return "";

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

  const normalized = raw
    .replace(/\\/g, "/")
    .replace(/^~\//, "/")
    .replace(/^wwwroot\//i, "/")
    .replace(/^public\//i, "/");

  if (normalized.startsWith("/")) {
    return `${baseByOwner[owner]}${normalized}`;
  }

  if (
    normalized.startsWith("uploads/") ||
    normalized.startsWith("images/") ||
    normalized.startsWith("img/") ||
    normalized.startsWith("photos/")
  ) {
    return `${baseByOwner[owner]}/${normalized}`;
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

export function pickFirstPhoto(source: any, keys: string[], fallbackOwner: MediaOwner = "auth") {
  for (const key of keys) {
    const value = source?.[key];

    if (value !== undefined && value !== null && String(value).trim() !== "") {
      return resolvePhotoUrl(String(value), fallbackOwner);
    }
  }

  return "";
}

export function escapeAttr(value: unknown) {
  return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}

export function escapeHtml(value: unknown) {
  return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
}

export function photoOrInitialsHtml(
  photo: string | null | undefined,
  title: string,
  className = "sh-photo",
  owner: MediaOwner = "auth"
) {
  const url = resolvePhotoUrl(photo, owner);
  const alt = escapeAttr(title || "photo");
  const letters = escapeHtml(initials(title));

  if (!url) {
    return `<div class="${className} sh-photo-fallback">${letters}</div>`;
  }

  return `
    <div class="${className}">
      <img src="${escapeAttr(url)}" alt="${alt}" loading="lazy" onerror="this.closest('.${className}')?.classList.add('sh-photo-broken'); this.remove();" />
      <span class="sh-photo-fallback-text">${letters}</span>
    </div>
  `;
}
