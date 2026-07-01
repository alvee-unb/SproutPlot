import { useCallback, useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import * as taskService from '../services/taskService'
import { ApiError } from '../services/apiClient'
import type { CreateTaskRequest, GardenTask, TaskType } from '../types/task'
import { TASK_TYPES, taskTypeLabel } from '../types/task'
import type { Plant } from '../types/plant'

function today(): string {
  return new Date().toISOString().slice(0, 10)
}

const emptyForm = (): CreateTaskRequest => ({ type: 'Water', title: '', plantId: null, dueOn: today(), notes: '' })

/** Task management for a single garden: add, list, complete, snooze, delete. */
export function GardenTasksPanel({ gardenId, plants }: { gardenId: string; plants: Plant[] }) {
  const [tasks, setTasks] = useState<GardenTask[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [form, setForm] = useState<CreateTaskRequest>(emptyForm)
  const [creating, setCreating] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const result = await taskService.listGardenTasks(gardenId)
      setTasks(result.items)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to load tasks.')
    } finally {
      setLoading(false)
    }
  }, [gardenId])

  useEffect(() => {
    void load()
  }, [load])

  async function handleCreate(event: FormEvent) {
    event.preventDefault()
    setCreating(true)
    setError(null)
    try {
      await taskService.createTask(gardenId, {
        type: form.type,
        title: form.title?.trim() || undefined,
        plantId: form.plantId || null,
        dueOn: form.dueOn,
        notes: form.notes?.trim() || undefined,
      })
      setForm(emptyForm())
      await load()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to add task.')
    } finally {
      setCreating(false)
    }
  }

  async function act(fn: () => Promise<unknown>) {
    setError(null)
    try {
      await fn()
      await load()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Action failed.')
    }
  }

  return (
    <section className="space-y-4">
      <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-50">Tasks</h2>

      <form
        onSubmit={handleCreate}
        className="grid gap-3 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:grid-cols-2 dark:border-slate-800 dark:bg-slate-900"
      >
        <label className="block">
          <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Type</span>
          <select
            value={form.type}
            onChange={(e) => setForm({ ...form, type: e.target.value as TaskType })}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
          >
            {TASK_TYPES.map((t) => (
              <option key={t} value={t}>
                {taskTypeLabel(t)}
              </option>
            ))}
          </select>
        </label>
        <label className="block">
          <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Due date</span>
          <input
            type="date"
            value={form.dueOn}
            onChange={(e) => setForm({ ...form, dueOn: e.target.value })}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
          />
        </label>
        <label className="block">
          <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Plant (optional)</span>
          <select
            value={form.plantId ?? ''}
            onChange={(e) => setForm({ ...form, plantId: e.target.value || null })}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
          >
            <option value="">Whole garden</option>
            {plants.map((p) => (
              <option key={p.id} value={p.id}>
                {p.name}
              </option>
            ))}
          </select>
        </label>
        <label className="block">
          <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Title (optional)</span>
          <input
            type="text"
            value={form.title ?? ''}
            onChange={(e) => setForm({ ...form, title: e.target.value })}
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
          />
        </label>
        <div className="sm:col-span-2">
          <button
            type="submit"
            disabled={creating}
            className="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:opacity-60"
          >
            {creating ? 'Adding…' : 'Add task'}
          </button>
        </div>
      </form>

      {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}

      {loading ? (
        <p className="text-sm text-slate-500 dark:text-slate-400">Loading…</p>
      ) : tasks.length === 0 ? (
        <p className="text-sm text-slate-500 dark:text-slate-400">No tasks yet.</p>
      ) : (
        <ul className="space-y-2">
          {tasks.map((task) => (
            <TaskRow
              key={task.id}
              task={task}
              onComplete={() => act(() => taskService.completeTask(task.id))}
              onSnooze={() => act(() => taskService.snoozeTask(task.id, 3))}
              onDelete={() => act(() => taskService.deleteTask(task.id))}
            />
          ))}
        </ul>
      )}
    </section>
  )
}

function TaskRow({
  task,
  onComplete,
  onSnooze,
  onDelete,
}: {
  task: GardenTask
  onComplete: () => void
  onSnooze: () => void
  onDelete: () => void
}) {
  const done = task.status === 'Completed'
  const overdue = !done && task.dueOn < today()

  return (
    <li className="flex items-center justify-between rounded-xl border border-slate-200 bg-white px-4 py-3 text-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="min-w-0">
        <span className={`font-medium ${done ? 'text-slate-400 line-through dark:text-slate-500' : 'text-slate-900 dark:text-slate-50'}`}>
          {task.title || taskTypeLabel(task.type)}
        </span>
        <span className="ml-2 text-slate-500 dark:text-slate-400">
          {taskTypeLabel(task.type)}
          {task.plantName ? ` · ${task.plantName}` : ''}
          {' · '}
          <span className={overdue ? 'font-medium text-red-600 dark:text-red-400' : ''}>
            {overdue ? 'Overdue ' : 'Due '}
            {task.dueOn}
          </span>
        </span>
      </div>
      <div className="ml-3 flex shrink-0 gap-3">
        {!done && (
          <>
            <button type="button" onClick={onComplete} className="text-xs font-medium text-emerald-700 hover:underline dark:text-emerald-400">
              Complete
            </button>
            <button type="button" onClick={onSnooze} className="text-xs font-medium text-slate-500 hover:underline dark:text-slate-400">
              Snooze 3d
            </button>
          </>
        )}
        <button type="button" onClick={onDelete} className="text-xs font-medium text-red-600 hover:underline dark:text-red-400">
          Delete
        </button>
      </div>
    </li>
  )
}
