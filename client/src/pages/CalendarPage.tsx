import { useCallback, useEffect, useMemo, useState } from 'react'
import * as calendarService from '../services/calendarService'
import { ApiError } from '../services/apiClient'
import type { CalendarEvent, CalendarEventKind } from '../types/calendar'

const WEEKDAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

function pad(n: number): string {
  return String(n).padStart(2, '0')
}

function isoDate(year: number, monthIndex: number, day: number): string {
  return `${year}-${pad(monthIndex + 1)}-${pad(day)}`
}

const kindClasses: Record<CalendarEventKind, string> = {
  Planting: 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900/50 dark:text-emerald-300',
  Task: 'bg-amber-100 text-amber-800 dark:bg-amber-900/50 dark:text-amber-300',
  Watering: 'bg-sky-100 text-sky-800 dark:bg-sky-900/50 dark:text-sky-300',
}

export function CalendarPage() {
  const now = new Date()
  const [year, setYear] = useState(now.getFullYear())
  const [monthIndex, setMonthIndex] = useState(now.getMonth())
  const [events, setEvents] = useState<CalendarEvent[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const daysInMonth = new Date(year, monthIndex + 1, 0).getDate()
      const from = isoDate(year, monthIndex, 1)
      const to = isoDate(year, monthIndex, daysInMonth)
      setEvents(await calendarService.getCalendar(from, to))
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to load calendar.')
    } finally {
      setLoading(false)
    }
  }, [year, monthIndex])

  useEffect(() => {
    void load()
  }, [load])

  // Group events by ISO date for quick per-day lookup.
  const eventsByDate = useMemo(() => {
    const map = new Map<string, CalendarEvent[]>()
    for (const event of events) {
      const list = map.get(event.date) ?? []
      list.push(event)
      map.set(event.date, list)
    }
    return map
  }, [events])

  const daysInMonth = new Date(year, monthIndex + 1, 0).getDate()
  const leadingBlanks = new Date(year, monthIndex, 1).getDay()
  const cells: (number | null)[] = [
    ...Array.from({ length: leadingBlanks }, () => null),
    ...Array.from({ length: daysInMonth }, (_, i) => i + 1),
  ]

  const monthLabel = new Date(year, monthIndex, 1).toLocaleDateString(undefined, { month: 'long', year: 'numeric' })
  const todayIso = isoDate(now.getFullYear(), now.getMonth(), now.getDate())

  function shiftMonth(delta: number) {
    const d = new Date(year, monthIndex + delta, 1)
    setYear(d.getFullYear())
    setMonthIndex(d.getMonth())
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-slate-900 dark:text-slate-50">Calendar</h1>
        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={() => shiftMonth(-1)}
            className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 transition hover:bg-slate-100 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
          >
            ← Prev
          </button>
          <span className="w-40 text-center text-sm font-medium text-slate-700 dark:text-slate-200">{monthLabel}</span>
          <button
            type="button"
            onClick={() => shiftMonth(1)}
            className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 transition hover:bg-slate-100 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
          >
            Next →
          </button>
        </div>
      </div>

      <div className="flex flex-wrap gap-4 text-xs text-slate-500 dark:text-slate-400">
        <span><span className="mr-1 inline-block h-2.5 w-2.5 rounded-full bg-emerald-400" />Planting</span>
        <span><span className="mr-1 inline-block h-2.5 w-2.5 rounded-full bg-amber-400" />Task</span>
        <span><span className="mr-1 inline-block h-2.5 w-2.5 rounded-full bg-sky-400" />Watering</span>
      </div>

      {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}
      {loading && <p className="text-sm text-slate-500 dark:text-slate-400">Loading…</p>}

      <div className="overflow-hidden rounded-2xl border border-slate-200 dark:border-slate-800">
        <div className="grid grid-cols-7 border-b border-slate-200 bg-slate-50 dark:border-slate-800 dark:bg-slate-900">
          {WEEKDAYS.map((day) => (
            <div key={day} className="px-2 py-2 text-center text-xs font-semibold text-slate-500 dark:text-slate-400">
              {day}
            </div>
          ))}
        </div>
        <div className="grid grid-cols-7">
          {cells.map((day, index) => {
            if (day === null) {
              return <div key={`blank-${index}`} className="min-h-24 border-b border-r border-slate-100 bg-slate-50/50 dark:border-slate-800 dark:bg-slate-900/40" />
            }
            const iso = isoDate(year, monthIndex, day)
            const dayEvents = eventsByDate.get(iso) ?? []
            const isToday = iso === todayIso
            return (
              <div key={iso} className="min-h-24 border-b border-r border-slate-100 p-1.5 dark:border-slate-800">
                <div
                  className={`mb-1 text-xs font-medium ${
                    isToday
                      ? 'inline-flex h-5 w-5 items-center justify-center rounded-full bg-emerald-600 text-white'
                      : 'text-slate-500 dark:text-slate-400'
                  }`}
                >
                  {day}
                </div>
                <div className="space-y-1">
                  {dayEvents.slice(0, 3).map((event) => (
                    <div
                      key={event.sourceId + event.kind}
                      title={`${event.title}${event.gardenName ? ` · ${event.gardenName}` : ''}${event.detail ? ` · ${event.detail}` : ''}`}
                      className={`truncate rounded px-1.5 py-0.5 text-[11px] ${kindClasses[event.kind]}`}
                    >
                      {event.title}
                    </div>
                  ))}
                  {dayEvents.length > 3 && (
                    <div className="px-1.5 text-[11px] text-slate-400">+{dayEvents.length - 3} more</div>
                  )}
                </div>
              </div>
            )
          })}
        </div>
      </div>
    </div>
  )
}
