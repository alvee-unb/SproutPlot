// Shapes exchanged with the authentication API. These mirror the backend DTOs.

export interface RegisterRequest {
  email: string
  password: string
  displayName?: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface AuthResponse {
  accessToken: string
  expiresAtUtc: string
  email: string
  displayName?: string | null
}

export interface CurrentUser {
  id: string
  email: string
}
