// Thin fetch wrapper: attaches the bearer token, sets JSON headers, and turns
// non-2xx responses into a typed ApiError with a human-readable message.

const TOKEN_KEY = 'sproutplot.token'

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token: string | null): void {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token)
  } else {
    localStorage.removeItem(TOKEN_KEY)
  }
}

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

interface ProblemDetails {
  title?: string
  detail?: string
  errors?: Record<string, string[]>
}

function messageFromProblem(body: ProblemDetails, fallback: string): string {
  if (body.errors) {
    const flattened = Object.values(body.errors).flat()
    if (flattened.length > 0) return flattened.join(' ')
  }
  return body.detail ?? body.title ?? fallback
}

export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = getToken()
  const headers = new Headers(options.headers)
  headers.set('Content-Type', 'application/json')
  if (token) headers.set('Authorization', `Bearer ${token}`)

  const response = await fetch(path, { ...options, headers })

  if (!response.ok) {
    const fallback = `Request failed (${response.status})`
    let message = fallback
    try {
      message = messageFromProblem((await response.json()) as ProblemDetails, fallback)
    } catch {
      // Non-JSON error body; keep the fallback message.
    }
    throw new ApiError(response.status, message)
  }

  if (response.status === 204) return undefined as T
  return (await response.json()) as T
}
