import { useCallback, useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { Link } from 'react-router-dom'
import * as gardenService from '../services/gardenService'
import { ApiError } from '../services/apiClient'
import type { CreateGardenRequest, Garden } from '../types/garden'

const emptyForm: CreateGardenRequest = { name: '', location: '', size: '', notes: '' }

export function GardensPage() {
  const [gardens, setGardens] = useState<Garden[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [search, setSearch] = useState('')
  const [form, setForm] = useState<CreateGardenRequest>(emptyForm)
  const [creating, setCreating] = useState(false)

  const load = useCallback(async (searchTerm: string) => {
    setLoading(true)
    setError(null)
    try {
      const result = await gardenService.listGardens({ search: searchTerm || undefined, sortBy: 'Name' })
      setGardens(result.items)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to load gardens.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void load(search)
    // Debounce-free: reload whenever the (trimmed) search term changes.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search])

  async function handleCreate(event: FormEvent) {
    event.preventDefault()
    setCreating(true)
    setError(null)
    try {
      await gardenService.createGarden({
        name: form.name.trim(),
        location: form.location?.trim() || undefined,
        size: form.size?.trim() || undefined,
        latitude: form.latitude ?? null,
        longitude: form.longitude ?? null,
        notes: form.notes?.trim() || undefined,
      })
      setForm(emptyForm)
      await load(search)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to create garden.')
    } finally {
      setCreating(false)
    }
  }

  async function handleUpdate(id: string, values: CreateGardenRequest) {
    await gardenService.updateGarden(id, values)
    await load(search)
  }

  async function handleDelete(id: string) {
    if (!window.confirm('Delete this garden? This cannot be undone.')) return
    await gardenService.deleteGarden(id)
    await load(search)
  }

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-slate-900 dark:text-slate-50">Your gardens</h1>
        <input
          type="search"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search by name…"
          className="w-48 rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
        />
      </div>

      {/* Create form */}
      <form
        onSubmit={handleCreate}
        className="grid gap-3 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:grid-cols-2 dark:border-slate-800 dark:bg-slate-900"
      >
        <h2 className="text-sm font-semibold text-slate-700 sm:col-span-2 dark:text-slate-300">Add a garden</h2>
        <Input label="Name" value={form.name} onChange={(v) => setForm({ ...form, name: v })} required />
        <Input label="Location" value={form.location ?? ''} onChange={(v) => setForm({ ...form, location: v })} />
        <Input label="Size" value={form.size ?? ''} onChange={(v) => setForm({ ...form, size: v })} />
        <Input label="Notes" value={form.notes ?? ''} onChange={(v) => setForm({ ...form, notes: v })} />
        <NumberInput label="Latitude" value={form.latitude} onChange={(v) => setForm({ ...form, latitude: v })} />
        <NumberInput label="Longitude" value={form.longitude} onChange={(v) => setForm({ ...form, longitude: v })} />
        <p className="text-xs text-slate-400 sm:col-span-2">
          Coordinates are optional — adding them enables rain-aware watering tips.
        </p>
        <div className="sm:col-span-2">
          <button
            type="submit"
            disabled={creating || !form.name.trim()}
            className="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:opacity-60"
          >
            {creating ? 'Adding…' : 'Add garden'}
          </button>
        </div>
      </form>

      {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}

      {/* List */}
      {loading ? (
        <p className="text-slate-500 dark:text-slate-400">Loading…</p>
      ) : gardens.length === 0 ? (
        <p className="text-slate-500 dark:text-slate-400">
          {search ? 'No gardens match your search.' : 'No gardens yet — add your first one above.'}
        </p>
      ) : (
        <ul className="space-y-3">
          {gardens.map((garden) => (
            <GardenCard key={garden.id} garden={garden} onSave={handleUpdate} onDelete={handleDelete} />
          ))}
        </ul>
      )}
    </div>
  )
}

// --- Sub-components ---

function GardenCard({
  garden,
  onSave,
  onDelete,
}: {
  garden: Garden
  onSave: (id: string, values: CreateGardenRequest) => Promise<void>
  onDelete: (id: string) => Promise<void>
}) {
  const [editing, setEditing] = useState(false)
  const [values, setValues] = useState<CreateGardenRequest>({
    name: garden.name,
    location: garden.location ?? '',
    size: garden.size ?? '',
    latitude: garden.latitude ?? undefined,
    longitude: garden.longitude ?? undefined,
    notes: garden.notes ?? '',
  })
  const [saving, setSaving] = useState(false)

  async function save(event: FormEvent) {
    event.preventDefault()
    setSaving(true)
    try {
      await onSave(garden.id, {
        name: values.name.trim(),
        location: values.location?.trim() || undefined,
        size: values.size?.trim() || undefined,
        latitude: values.latitude ?? null,
        longitude: values.longitude ?? null,
        notes: values.notes?.trim() || undefined,
      })
      setEditing(false)
    } finally {
      setSaving(false)
    }
  }

  if (editing) {
    return (
      <li className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <form onSubmit={save} className="grid gap-3 sm:grid-cols-2">
          <Input label="Name" value={values.name} onChange={(v) => setValues({ ...values, name: v })} required />
          <Input label="Location" value={values.location ?? ''} onChange={(v) => setValues({ ...values, location: v })} />
          <Input label="Size" value={values.size ?? ''} onChange={(v) => setValues({ ...values, size: v })} />
          <Input label="Notes" value={values.notes ?? ''} onChange={(v) => setValues({ ...values, notes: v })} />
          <NumberInput label="Latitude" value={values.latitude} onChange={(v) => setValues({ ...values, latitude: v })} />
          <NumberInput label="Longitude" value={values.longitude} onChange={(v) => setValues({ ...values, longitude: v })} />
          <div className="flex gap-2 sm:col-span-2">
            <button
              type="submit"
              disabled={saving || !values.name.trim()}
              className="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:opacity-60"
            >
              {saving ? 'Saving…' : 'Save'}
            </button>
            <button
              type="button"
              onClick={() => setEditing(false)}
              className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-100 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
            >
              Cancel
            </button>
          </div>
        </form>
      </li>
    )
  }

  return (
    <li className="flex items-start justify-between rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="min-w-0">
        <Link
          to={`/gardens/${garden.id}`}
          className="font-semibold text-slate-900 hover:text-emerald-600 hover:underline dark:text-slate-50 dark:hover:text-emerald-400"
        >
          {garden.name}
        </Link>
        <p className="mt-0.5 text-sm text-slate-500 dark:text-slate-400">
          {[garden.location, garden.size].filter(Boolean).join(' · ') || 'No location or size set'}
        </p>
        {garden.notes && <p className="mt-2 text-sm text-slate-600 dark:text-slate-300">{garden.notes}</p>}
      </div>
      <div className="ml-4 flex shrink-0 gap-2">
        <button
          type="button"
          onClick={() => setEditing(true)}
          className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 transition hover:bg-slate-100 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
        >
          Edit
        </button>
        <button
          type="button"
          onClick={() => onDelete(garden.id)}
          className="rounded-lg border border-red-200 px-3 py-1.5 text-sm font-medium text-red-600 transition hover:bg-red-50 dark:border-red-900 dark:text-red-400 dark:hover:bg-red-950/40"
        >
          Delete
        </button>
      </div>
    </li>
  )
}

function Input({
  label,
  value,
  onChange,
  required = false,
}: {
  label: string
  value: string
  onChange: (value: string) => void
  required?: boolean
}) {
  return (
    <label className="block">
      <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">{label}</span>
      <input
        type="text"
        value={value}
        required={required}
        onChange={(e) => onChange(e.target.value)}
        className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
      />
    </label>
  )
}

function NumberInput({
  label,
  value,
  onChange,
}: {
  label: string
  value?: number | null
  onChange: (value: number | null) => void
}) {
  return (
    <label className="block">
      <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">{label}</span>
      <input
        type="number"
        step="any"
        value={value ?? ''}
        onChange={(e) => onChange(e.target.value === '' ? null : Number(e.target.value))}
        className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
      />
    </label>
  )
}
