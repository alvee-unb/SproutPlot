import { apiFetch } from './apiClient'
import type {
  AuthResponse,
  CurrentUser,
  LoginRequest,
  RegisterRequest,
} from '../types/auth'

// Auth API calls. Kept free of React concerns so they are easy to test/reuse.

export function register(request: RegisterRequest): Promise<AuthResponse> {
  return apiFetch<AuthResponse>('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export function login(request: LoginRequest): Promise<AuthResponse> {
  return apiFetch<AuthResponse>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export function getCurrentUser(): Promise<CurrentUser> {
  return apiFetch<CurrentUser>('/api/auth/me')
}
