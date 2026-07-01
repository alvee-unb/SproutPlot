import { useCallback, useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import * as wateringService from '../services/wateringService'
import { ApiError } from '../services/apiClient'
import type { WateringEvent, WateringRecommendation } from '../types/watering'

/** Watering guidance, quick logging, and recent history for a garden. */
export function WateringPanel({ gardenId }: { gardenId: string }) {
  const [recommendation, setRecommendation] = useState<WateringRecommendation | null>(null)
  const [history, setHistory] = useState<WateringEvent[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [notes, setNotes] = useState('')
  const [amount, setAmount] = useState('')
  const [logging, setLogging] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const [rec, events] = await Promise.all([
        wateringService.getRecommendation(gardenId),
        wateringService.listWaterings(gardenId),
      ])
      setRecommendation(rec)
      setHistory(events.items)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to load watering data.')
    } finally {
      setLoading(false)
    }
  }, [gardenId])

  useEffect(() => {
    void load()
  }, [load])

  async function logWatering(event: FormEvent) {
    event.preventDefault()
    setLogging(true)
    setError(null)
    try {
      await wateringService.recordWatering(gardenId, {
        notes: notes.trim() || undefined,
        amountLiters: amount === '' ? null : Number(amount),
      })
      setNotes('')
      setAmount('')
      await load()
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to log watering.')
    } finally {
      setLogging(false)
    }
  }

  async function remove(id: string) {
    await wateringService.deleteWatering(id)
    await load()
  }

  return (
    <section className="space-y-4">
      <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-50">Watering</h2>

      {loading && <p className="text-sm text-slate-500 dark:text-slate-400">Loading…</p>}
      {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}

      {recommendation && (
        <div
          className={`rounded-2xl border p-4 ${
            recommendation.shouldWaterNow
              ? 'border-amber-200 bg-amber-50 dark:border-amber-900 dark:bg-amber-950/30'
              : 'border-emerald-200 bg-emerald-50 dark:border-emerald-900 dark:bg-emerald-950/30'
          }`}
        >
          <p className="font-medium text-slate-900 dark:text-slate-50">
            {recommendation.shouldWaterNow ? '💧 Time to water' : '✅ No watering needed right now'}
          </p>
          <p className="mt-1 text-sm text-slate-600 dark:text-slate-300">{recommendation.reason}</p>
          <p className="mt-2 text-xs text-slate-500 dark:text-slate-400">
            {recommendation.season} · waters every ~{recommendation.effectiveIntervalDays} days
            {recommendation.rainConsidered ? ' · rain forecast considered' : ' · add garden coordinates for rain-aware tips'}
          </p>
        </div>
      )}

      <form onSubmit={logWatering} className="flex flex-wrap items-end gap-2">
        <label className="block">
          <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Amount (L)</span>
          <input
            type="number"
            step="any"
            min={0}
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            className="w-24 rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
          />
        </label>
        <label className="block flex-1">
          <span className="mb-1 block text-xs font-medium text-slate-600 dark:text-slate-400">Notes</span>
          <input
            type="text"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            placeholder="Optional"
            className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
          />
        </label>
        <button
          type="submit"
          disabled={logging}
          className="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:opacity-60"
        >
          {logging ? 'Logging…' : 'Log watering'}
        </button>
      </form>

      {history.length > 0 && (
        <ul className="divide-y divide-slate-100 rounded-2xl border border-slate-200 dark:divide-slate-800 dark:border-slate-800">
          {history.map((event) => (
            <li key={event.id} className="flex items-center justify-between px-4 py-2.5 text-sm">
              <div className="min-w-0">
                <span className="text-slate-700 dark:text-slate-200">
                  {new Date(event.wateredAtUtc).toLocaleDateString()} · {event.plantName ?? 'Whole garden'}
                </span>
                {(event.amountLiters != null || event.notes) && (
                  <span className="text-slate-400">
                    {event.amountLiters != null ? ` · ${event.amountLiters} L` : ''}
                    {event.notes ? ` · ${event.notes}` : ''}
                  </span>
                )}
              </div>
              <button
                type="button"
                onClick={() => remove(event.id)}
                className="ml-3 shrink-0 text-xs font-medium text-red-600 hover:underline dark:text-red-400"
              >
                Remove
              </button>
            </li>
          ))}
        </ul>
      )}
    </section>
  )
}
