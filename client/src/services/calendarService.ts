import { apiFetch } from './apiClient'
import type { CalendarEvent } from '../types/calendar'

export function getCalendar(from: string, to: string, gardenId?: string): Promise<CalendarEvent[]> {
  const search = new URLSearchParams({ from, to })
  if (gardenId) search.set('gardenId', gardenId)
  return apiFetch<CalendarEvent[]>(`/api/calendar?${search.toString()}`)
}
