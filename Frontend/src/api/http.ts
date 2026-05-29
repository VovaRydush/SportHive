export const AUTH_API_URL =
  "http://localhost:5154";

export const COMMAND_API_URL =
  "http://localhost:5123";

export const EVENT_API_URL =
  "http://localhost:5042";

type ApiOptions = RequestInit & {
  auth?: boolean;
  baseUrl?: string;
};

function getToken() {
  const token = localStorage.getItem("accessToken");
  return token ? token.replace(/^"(.+)"$/, "$1") : "";
}

async function parseResponse<T>(response: Response): Promise<T> {
  const contentType = response.headers.get("content-type") || "";

  if (response.status === 204) return undefined as T;

  if (contentType.includes("application/json")) return response.json();

  const text = await response.text();
  try { return JSON.parse(text) as T; } catch { return text as T; }
}

export async function apiRequest<T>(endpoint: string, options: ApiOptions = {}): Promise<T> {
  const baseUrl = options.baseUrl || AUTH_API_URL;
  const token = getToken();
  const isFormData = options.body instanceof FormData;
  const { baseUrl: _, auth, ...fetchOptions } = options;

  const response = await fetch(`${baseUrl}${endpoint}`, {
    ...fetchOptions,
    headers: {
      ...(isFormData ? {} : { "Content-Type": "application/json" }),
      ...(auth !== false && token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {}),
    },
  });

  if (!response.ok) {
    const errorBody = await parseResponse<any>(response);
    if (typeof errorBody === "string") throw new Error(errorBody || `HTTP error ${response.status}`);
    throw new Error(errorBody?.detail || errorBody?.message || errorBody?.title || `HTTP error ${response.status}`);
  }

  return parseResponse<T>(response);
}
