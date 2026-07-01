import { useCallback, useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useParams } from 'react-router-dom'
import * as gardenService from '../services/gardenService'
import * as plantService from '../services/plantService'
import { ApiError } from '../services/apiClient'
import type { Garden } from '../types/garden'
import { PLANT_STATUSES } from '../types/plant'
import type { Plant, PlantInput, PlantStatus, PlantType } from '../types/plant'

const emptyPlant: PlantInput = {
  name: '',
  plantTypeId: null,
  variety: '',
  datePlanted: null,
  quantity: 1,
  status: 'Growing',
  notes: '',
}

export function GardenDetailPage() {
  const { id = '' } = useParams()
  const [garden, setGarden] = useState<Garden | null>(null)
  const [plants, setPlants] = useState<Plant[]>([])
  const [types, setTypes] = useState<PlantType[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [form, setForm] = useState<PlantInput>(emptyPlant)
  const [creating, setCreating] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const [g, p, t] = await Promise.all([
        gardenService.getGarden(id),
        plantService.listPlants(id),
        plantService.listPlantTypes(),
      ])
      setGarden(g)
      setPlants(p.items)
      setTypes(t)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to load garden.')
    } finally {
      setLoading(false)
    }
  }, [id])

  useEffect(() => {
    void load()
  }, [load])

  async function handleCreate(event: FormEvent) {
    event.preventDefault()
    setCreating(true)
    setError(null)
    try {
      await plantService.createPlant(id, normalise(form))
      setForm(emptyPlant)
      await reloadPlants()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to add plant.')
    } finally {
      setCreating(false)
    }
  }

  async function reloadPlants() {
    const p = await plantService.listPlants(id)
    setPlants(p.items)
  }

  async function handleUpdate(plantId: string, values: PlantInput) {
    await plantService.updatePlant(plantId, normalise(values))
    await reloadPlants()
  }

  async function handleDelete(plantId: string) {
    if (!window.confirm('Delete this plant?')) return
    await plantService.deletePlant(plantId)
    await reloadPlants()
  }

  if (loading) return <p className="text-slate-500 dark:text-slate-400">Loading…</p>

  if (!garden) {
    return (
      <div className="space-y-4">
        <p className="text-slate-600 dark:text-slate-400">{error ?? 'Garden not found.'}</p>
        <Link to="/gardens" className="text-emerald-600 hover:underline dark:text-emerald-400">
          ← Back to gardens
        </Link>
      </div>
    )
  }

  return (
    <div className="space-y-8">
      <div>
        <Link to="/gardens" className="text-sm text-emerald-600 hover:underline dark:text-emerald-400">
          ← Back to gardens
        </Link>
        <h1 className="mt-2 text-2xl font-semibold text-slate-900 dark:text-slate-50">{garden.name}</h1>
        <p className="text-sm text-slate-500 dark:text-slate-400">
          {[garden.location, garden.size].filter(Boolean).join(' · ') || 'No location or size set'}
        </p>
      </div>

      <PlantForm
        title="Add a plant"
        types={types}
        values={form}
        onChange={setForm}
        onSubmit={handleCreate}
        submitting={creating}
        submitLabel="Add plant"
      />

      {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}

      <section>
        <h2 className="mb-3 text-lg font-semibold text-slate-900 dark:text-slate-50">
          Plants ({plants.length})
        </h2>
        {plants.length === 0 ? (
          <p className="text-slate-500 dark:text-slate-400">No plants yet — add your first above.</p>
        ) : (
          <ul className="space-y-3">
            {plants.map((plant) => (
              <PlantCard
                key={plant.id}
                plant={plant}
                types={types}
                onSave={handleUpdate}
                onDelete={handleDelete}
              />
            ))}
          </ul>
        )}
      </section>
    </div>
  )
}

function normalise(input: PlantInput): PlantInput {
  return {
    ...input,
    name: input.name.trim(),
    variety: input.variety?.trim() || undefined,
    notes: input.notes?.trim() || undefined,
    plantTypeId: input.plantTypeId || null,
    datePlanted: input.datePlanted || null,
    quantity: Number(input.quantity) || 1,
  }
}

// --- Sub-components ---

function statusClasses(status: PlantStatus): string {
  switch (status) {
    case 'Growing':
      return 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900/50 dark:text-emerald-300'
    case 'Harvested':
      return 'bg-amber-100 text-amber-800 dark:bg-amber-900/50 dark:text-amber-300'
    case 'Planned':
      return 'bg-sky-100 text-sky-800 dark:bg-sky-900/50 dark:text-sky-300'
    default:
      return 'bg-slate-200 text-slate-700 dark:bg-slate-700 dark:text-slate-200'
  }
}

function PlantCard({
  plant,
  types,
  onSave,
  onDelete,
}: {
  plant: Plant
  types: PlantType[]
  onSave: (id: string, values: PlantInput) => Promise<void>
  onDelete: (id: string) => Promise<void>
}) {
  const [editing, setEditing] = useState(false)
  const [values, setValues] = useState<PlantInput>({
    name: plant.name,
    plantTypeId: plant.plantTypeId ?? null,
    variety: plant.variety ?? '',
    datePlanted: plant.datePlanted ?? null,
    quantity: plant.quantity,
    status: plant.status,
    notes: plant.notes ?? '',
  })
  const [saving, setSaving] = useState(false)

  async function save(event: FormEvent) {
    event.preventDefault()
    setSaving(true)
    try {
      await onSave(plant.id, values)
      setEditing(false)
    } finally {
      setSaving(false)
    }
  }

  if (editing) {
    return (
      <li className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <PlantForm
          types={types}
          values={values}
          onChange={setValues}
          onSubmit={save}
          submitting={saving}
          submitLabel="Save"
          onCancel={() => setEditing(false)}
        />
      </li>
    )
  }

  const details = [plant.variety, `Qty ${plant.quantity}`, plant.datePlanted ? `Planted ${plant.datePlanted}` : null]
    .filter(Boolean)
    .join(' · ')

  return (
    <li className="flex items-start justify-between rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="min-w-0">
        <div className="flex items-center gap-2">
          <h3 className="font-semibold text-slate-900 dark:text-slate-50">{plant.name}</h3>
          <span className={`rounded-full px-2 py-0.5 text-xs font-medium ${statusClasses(plant.status)}`}>
            {plant.status}
          </span>
        </div>
        <p className="mt-0.5 text-sm text-slate-500 dark:text-slate-400">
          {plant.plantTypeName ? `${plant.plantTypeName}${details ? ' · ' : ''}` : ''}
          {details}
        </p>
        {plant.notes && <p className="mt-2 text-sm text-slate-600 dark:text-slate-300">{plant.notes}</p>}
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
          onClick={() => onDelete(plant.id)}
          className="rounded-lg border border-red-200 px-3 py-1.5 text-sm font-medium text-red-600 transition hover:bg-red-50 dark:border-red-900 dark:text-red-400 dark:hover:bg-red-950/40"
        >
          Delete
        </button>
      </div>
    </li>
  )
}

function PlantForm({
  title,
  types,
  values,
  onChange,
  onSubmit,
  submitting,
  submitLabel,
  onCancel,
}: {
  title?: string
  types: PlantType[]
  values: PlantInput
  onChange: (values: PlantInput) => void
  onSubmit: (event: FormEvent) => void
  submitting: boolean
  submitLabel: string
  onCancel?: () => void
}) {
  return (
    <form
      onSubmit={onSubmit}
      className="grid gap-3 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:grid-cols-2 dark:border-slate-800 dark:bg-slate-900"
    >
      {title && <h2 className="text-sm font-semibold text-slate-700 sm:col-span-2 dark:text-slate-300">{title}</h2>}

      <Text label="Name" value={values.name} onChange={(v) => onChange({ ...values, name: v })} required />

      <label className="block">
        <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Type</span>
        <select
          value={values.plantTypeId ?? ''}
          onChange={(e) => onChange({ ...values, plantTypeId: e.target.value || null })}
          className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
        >
          <option value="">— none —</option>
          {types.map((t) => (
            <option key={t.id} value={t.id}>
              {t.name}
            </option>
          ))}
        </select>
      </label>

      <Text label="Variety" value={values.variety ?? ''} onChange={(v) => onChange({ ...values, variety: v })} />

      <label className="block">
        <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Quantity</span>
        <input
          type="number"
          min={1}
          value={values.quantity}
          onChange={(e) => onChange({ ...values, quantity: Number(e.target.value) })}
          className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
        />
      </label>

      <label className="block">
        <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Date planted</span>
        <input
          type="date"
          value={values.datePlanted ?? ''}
          onChange={(e) => onChange({ ...values, datePlanted: e.target.value || null })}
          className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
        />
      </label>

      <label className="block">
        <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Status</span>
        <select
          value={values.status}
          onChange={(e) => onChange({ ...values, status: e.target.value as PlantStatus })}
          className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
        >
          {PLANT_STATUSES.map((s) => (
            <option key={s} value={s}>
              {s}
            </option>
          ))}
        </select>
      </label>

      <Text
        label="Notes"
        value={values.notes ?? ''}
        onChange={(v) => onChange({ ...values, notes: v })}
        className="sm:col-span-2"
      />

      <div className="flex gap-2 sm:col-span-2">
        <button
          type="submit"
          disabled={submitting || !values.name.trim()}
          className="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {submitting ? 'Saving…' : submitLabel}
        </button>
        {onCancel && (
          <button
            type="button"
            onClick={onCancel}
            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-100 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
          >
            Cancel
          </button>
        )}
      </div>
    </form>
  )
}

function Text({
  label,
  value,
  onChange,
  required = false,
  className = '',
}: {
  label: string
  value: string
  onChange: (value: string) => void
  required?: boolean
  className?: string
}) {
  return (
    <label className={`block ${className}`}>
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
