import { apiFetch } from './apiClient'
import type { PagedResult } from '../types/garden'
import type { Plant, PlantInput, PlantType } from '../types/plant'

export function listPlants(gardenId: string): Promise<PagedResult<Plant>> {
  return apiFetch<PagedResult<Plant>>(`/api/gardens/${gardenId}/plants?sortBy=Name&pageSize=100`)
}

export function createPlant(gardenId: string, input: PlantInput): Promise<Plant> {
  return apiFetch<Plant>(`/api/gardens/${gardenId}/plants`, {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export function updatePlant(id: string, input: PlantInput): Promise<Plant> {
  return apiFetch<Plant>(`/api/plants/${id}`, { method: 'PUT', body: JSON.stringify(input) })
}

export function deletePlant(id: string): Promise<void> {
  return apiFetch<void>(`/api/plants/${id}`, { method: 'DELETE' })
}

export function listPlantTypes(): Promise<PlantType[]> {
  return apiFetch<PlantType[]>('/api/plant-types')
}
