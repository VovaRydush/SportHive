const API_BASE_URL = "http://localhost:5154";

type RequestOptions = RequestInit & {
  auth?: boolean;
};

export async function apiRequest<T>(
  url: string,
  options: RequestOptions = {}
): Promise<T> {
  const token = localStorage.getItem("accessToken");

  const headers: HeadersInit = {
    ...(options.body instanceof FormData ? {} : { "Content-Type": "application/json" }),
    ...(options.auth !== false && token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  };

  const response = await fetch(`${API_BASE_URL}${url}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Помилка API: ${response.status}`);
  }

  const contentType = response.headers.get("content-type");

  if (!contentType?.includes("application/json")) {
    return (await response.text()) as T;
  }

  return response.json();
}