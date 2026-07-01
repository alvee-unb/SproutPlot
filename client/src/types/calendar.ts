export type CalendarEventKind = 'Planting' | 'Task' | 'Watering'

export interface CalendarEvent {
  date: string
  kind: CalendarEventKind
  title: string
  gardenId: string
  gardenName?: string | null
  plantId?: string | null
  plantName?: string | null
  detail?: string | null
  sourceId: string
}
