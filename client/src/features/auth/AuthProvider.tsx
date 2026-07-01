import { useCallback, useEffect, useMemo, useState } from 'react'
import type { ReactNode } from 'react'
import * as authService from '../../services/authService'
import { setToken } from '../../services/apiClient'
import type { CurrentUser, LoginRequest, RegisterRequest } from '../../types/auth'
import { AuthContext } from './authContext'

/**
 * Holds authentication state for the app. On mount it tries to restore the
 * session from a stored token by calling /me; if that fails the token is
 * cleared. Token storage is localStorage (simple; a refresh-token flow can be
 * layered on later without changing this contract).
 */
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false

    authService
      .getCurrentUser()
      .then((restored) => {
        if (!cancelled) setUser(restored)
      })
      .catch(() => {
        // No valid token / expired session — remain signed out.
        setToken(null)
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [])

  const login = useCallback(async (request: LoginRequest) => {
    const response = await authService.login(request)
    setToken(response.accessToken)
    setUser(await authService.getCurrentUser())
  }, [])

  const register = useCallback(async (request: RegisterRequest) => {
    const response = await authService.register(request)
    setToken(response.accessToken)
    setUser(await authService.getCurrentUser())
  }, [])

  const logout = useCallback(() => {
    setToken(null)
    setUser(null)
  }, [])

  const value = useMemo(
    () => ({ user, isAuthenticated: user !== null, loading, login, register, logout }),
    [user, loading, login, register, logout],
  )

  return <AuthContext value={value}>{children}</AuthContext>
}
