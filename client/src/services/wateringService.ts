import { apiFetch } from './apiClient'
import type { PagedResult } from '../types/garden'
import type { RecordWateringRequest, WateringEvent, WateringRecommendation } from '../types/watering'

export function getRecommendation(gardenId: string): Promise<WateringRecommendation> {
  return apiFetch<WateringRecommendation>(`/api/gardens/${gardenId}/watering-recommendation`)
}

export function listWaterings(gardenId: string): Promise<PagedResult<WateringEvent>> {
  return apiFetch<PagedResult<WateringEvent>>(`/api/gardens/${gardenId}/waterings?pageSize=20`)
}

export function recordWatering(gardenId: string, request: RecordWateringRequest): Promise<WateringEvent> {
  return apiFetch<WateringEvent>(`/api/gardens/${gardenId}/waterings`, {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export function deleteWatering(id: string): Promise<void> {
  return apiFetch<void>(`/api/waterings/${id}`, { method: 'DELETE' })
}
