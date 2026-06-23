export const AUTH_API_URL = "http://localhost:5154";
export const COMMAND_API_URL = "http://localhost:5123";
export const EVENT_API_URL = "http://localhost:5042";

type ApiOptions = RequestInit & {
  auth?: boolean;
  baseUrl?: string;
};

function getToken() {
  return (
    localStorage.getItem("accessToken") ||
    localStorage.getItem("token") ||
    localStorage.getItem("jwt") ||
    ""
  )
    .replace(/^"(.+)"$/, "$1")
    .replace(/^Bearer\s+/i, "");
}

async function parseResponse<T>(response: Response): Promise<T> {
  if (response.status === 204) return undefined as T;

  const contentType = response.headers.get("content-type") || "";

  if (contentType.includes("application/json")) {
    return response.json() as Promise<T>;
  }

  const text = await response.text();

  try {
    return JSON.parse(text) as T;
  } catch {
    return text as T;
  }
}

export async function apiRequest<T = any>(endpoint: string, options: ApiOptions = {}): Promise<T> {
  const baseUrl = options.baseUrl || AUTH_API_URL;
  const token = getToken();
  const isFormData = options.body instanceof FormData;
  const { baseUrl: ignoredBaseUrl, auth, headers, ...fetchOptions } = options;

  const response = await fetch(`${baseUrl}${endpoint}`, {
    ...fetchOptions,
    headers: {
      ...(isFormData ? {} : { "Content-Type": "application/json" }),
      ...(auth !== false && token ? { Authorization: `Bearer ${token}` } : {}),
      ...(headers || {}),
    },
  });

  if (!response.ok) {
    const errorBody = await parseResponse<any>(response);

    if (typeof errorBody === "string") {
      throw new Error(errorBody || `HTTP error ${response.status}`);
    }

    throw new Error(
      errorBody?.detail ||
      errorBody?.message ||
      errorBody?.title ||
      errorBody?.error ||
      `HTTP error ${response.status}`
    );
  }

  return parseResponse<T>(response);
}
