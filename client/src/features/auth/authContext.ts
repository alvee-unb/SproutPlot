import { createContext } from 'react'
import type { CurrentUser, LoginRequest, RegisterRequest } from '../../types/auth'

export interface AuthContextValue {
  user: CurrentUser | null
  isAuthenticated: boolean
  /** True while the initial token-based session restore is in flight. */
  loading: boolean
  login: (request: LoginRequest) => Promise<void>
  register: (request: RegisterRequest) => Promise<void>
  logout: () => void
}

// Defined in its own module (no component export) so React Fast Refresh stays happy.
export const AuthContext = createContext<AuthContextValue | undefined>(undefined)
