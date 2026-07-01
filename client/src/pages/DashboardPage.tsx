import { Link } from 'react-router-dom'
import { useAuth } from '../features/auth/useAuth'

/**
 * Landing page after sign-in. Weather, today's tasks, and plants needing
 * attention will be added in later slices; for now it points to Gardens.
 */
export function DashboardPage() {
  const { user } = useAuth()

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold text-slate-900 dark:text-slate-50">
        Welcome back{user?.email ? `, ${user.email}` : ''} 🌱
      </h1>
      <p className="text-slate-600 dark:text-slate-400">
        Your garden dashboard is taking shape. Start by setting up your gardens.
      </p>
      <Link
        to="/gardens"
        className="inline-block rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700"
      >
        Manage gardens
      </Link>
    </div>
  )
}
