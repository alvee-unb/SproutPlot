import { Link } from 'react-router-dom'
import { useAuth } from '../features/auth/useAuth'
import { WeatherCard } from '../components/WeatherCard'

/**
 * Landing page after sign-in. Shows local weather; today's tasks and plants
 * needing attention will be added in later slices.
 */
export function DashboardPage() {
  const { user } = useAuth()

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold text-slate-900 dark:text-slate-50">
          Welcome back{user?.email ? `, ${user.email}` : ''} 🌱
        </h1>
        <p className="mt-1 text-slate-600 dark:text-slate-400">Here's your garden at a glance.</p>
      </div>

      <WeatherCard />

      <Link
        to="/gardens"
        className="inline-block rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700"
      >
        Manage gardens
      </Link>
    </div>
  )
}
