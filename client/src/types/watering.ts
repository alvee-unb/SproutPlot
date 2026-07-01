export type Season = 'Spring' | 'Summer' | 'Autumn' | 'Winter'

export interface WateringRecommendation {
  shouldWaterNow: boolean
  reason: string
  effectiveIntervalDays: number
  daysSinceLastWatering?: number | null
  nextDueDate?: string | null
  season: Season
  rainConsidered: boolean
}

export interface WateringEvent {
  id: string
  gardenId: string
  plantId?: string | null
  plantName?: string | null
  wateredAtUtc: string
  amountLiters?: number | null
  notes?: string | null
}

export interface RecordWateringRequest {
  plantId?: string | null
  wateredAtUtc?: string | null
  amountLiters?: number | null
  notes?: string
}
