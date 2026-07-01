import { useAuth } from '../features/auth/useAuth'

/**
 * Placeholder dashboard confirming an authenticated session. The real
 * dashboard (weather, tasks, garden overview) arrives in later slices.
 */
export function DashboardPage() {
  const { user, logout } = useAuth()

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-950">
      <header className="flex items-center justify-between border-b border-slate-200 bg-white px-6 py-4 dark:border-slate-800 dark:bg-slate-900">
        <span className="text-lg font-semibold text-emerald-600 dark:text-emerald-400">SproutPlot</span>
        <div className="flex items-center gap-4">
          <span className="text-sm text-slate-500 dark:text-slate-400">{user?.email}</span>
          <button
            type="button"
            onClick={logout}
            className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 transition hover:bg-slate-100 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
          >
            Sign out
          </button>
        </div>
      </header>

      <main className="mx-auto max-w-4xl px-6 py-12">
        <h1 className="text-2xl font-semibold text-slate-900 dark:text-slate-50">
          You're signed in 🌱
        </h1>
        <p className="mt-2 text-slate-600 dark:text-slate-400">
          Your garden dashboard will live here — weather, today's tasks, and plants needing
          attention are coming in upcoming features.
        </p>
      </main>
    </div>
  )
}
