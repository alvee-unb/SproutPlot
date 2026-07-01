import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import * as taskService from '../services/taskService'
import { ApiError } from '../services/apiClient'
import type { GardenTask } from '../types/task'
import { taskTypeLabel } from '../types/task'

function addDays(days: number): string {
  const d = new Date()
  d.setDate(d.getDate() + days)
  return d.toISOString().slice(0, 10)
}

/** Dashboard widget: pending tasks due soon, grouped overdue / today / upcoming. */
export function DashboardTasks() {
  const [tasks, setTasks] = useState<GardenTask[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const result = await taskService.listTasks({ status: 'Pending', dueOnOrBefore: addDays(7) })
      setTasks(result.items)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to load tasks.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void load()
  }, [load])

  async function complete(id: string) {
    await taskService.completeTask(id)
    await load()
  }

  async function snooze(id: string) {
    await taskService.snoozeTask(id, 3)
    await load()
  }

  const todayStr = new Date().toISOString().slice(0, 10)
  const overdue = tasks.filter((t) => t.dueOn < todayStr)
  const dueToday = tasks.filter((t) => t.dueOn === todayStr)
  const upcoming = tasks.filter((t) => t.dueOn > todayStr)

  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <h2 className="mb-4 text-lg font-semibold text-slate-900 dark:text-slate-50">Tasks</h2>

      {loading && <p className="text-sm text-slate-500 dark:text-slate-400">Loading…</p>}
      {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}

      {!loading && tasks.length === 0 && (
        <p className="text-sm text-slate-500 dark:text-slate-400">
          Nothing due in the next week. Add tasks from a{' '}
          <Link to="/gardens" className="text-emerald-600 hover:underline dark:text-emerald-400">
            garden
          </Link>
          .
        </p>
      )}

      <div className="space-y-4">
        <TaskGroup title="Overdue" tone="danger" tasks={overdue} onComplete={complete} onSnooze={snooze} />
        <TaskGroup title="Today" tone="accent" tasks={dueToday} onComplete={complete} onSnooze={snooze} />
        <TaskGroup title="Upcoming" tone="muted" tasks={upcoming} onComplete={complete} onSnooze={snooze} />
      </div>
    </section>
  )
}

function TaskGroup({
  title,
  tone,
  tasks,
  onComplete,
  onSnooze,
}: {
  title: string
  tone: 'danger' | 'accent' | 'muted'
  tasks: GardenTask[]
  onComplete: (id: string) => void
  onSnooze: (id: string) => void
}) {
  if (tasks.length === 0) return null

  const toneClass =
    tone === 'danger'
      ? 'text-red-600 dark:text-red-400'
      : tone === 'accent'
        ? 'text-emerald-700 dark:text-emerald-400'
        : 'text-slate-500 dark:text-slate-400'

  return (
    <div>
      <h3 className={`mb-1.5 text-xs font-semibold uppercase tracking-wide ${toneClass}`}>
        {title} ({tasks.length})
      </h3>
      <ul className="space-y-1.5">
        {tasks.map((task) => (
          <li key={task.id} className="flex items-center justify-between gap-3 text-sm">
            <span className="min-w-0 truncate text-slate-700 dark:text-slate-200">
              {task.title || taskTypeLabel(task.type)}
              <span className="text-slate-400">
                {' · '}
                {task.gardenName}
                {task.plantName ? ` · ${task.plantName}` : ''}
                {` · ${task.dueOn}`}
              </span>
            </span>
            <span className="flex shrink-0 gap-3">
              <button
                type="button"
                onClick={() => onComplete(task.id)}
                className="text-xs font-medium text-emerald-700 hover:underline dark:text-emerald-400"
              >
                Done
              </button>
              <button
                type="button"
                onClick={() => onSnooze(task.id)}
                className="text-xs font-medium text-slate-500 hover:underline dark:text-slate-400"
              >
                Snooze
              </button>
            </span>
          </li>
        ))}
      </ul>
    </div>
  )
}
