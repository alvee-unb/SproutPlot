import { apiFetch } from './apiClient'
import type {
  CreateGardenRequest,
  Garden,
  GardenListParams,
  PagedResult,
  UpdateGardenRequest,
} from '../types/garden'

function buildQuery(params: GardenListParams): string {
  const search = new URLSearchParams()
  if (params.search) search.set('search', params.search)
  if (params.sortBy) search.set('sortBy', params.sortBy)
  if (params.descending) search.set('descending', 'true')
  if (params.page) search.set('page', String(params.page))
  if (params.pageSize) search.set('pageSize', String(params.pageSize))
  const qs = search.toString()
  return qs ? `?${qs}` : ''
}

export function listGardens(params: GardenListParams = {}): Promise<PagedResult<Garden>> {
  return apiFetch<PagedResult<Garden>>(`/api/gardens${buildQuery(params)}`)
}

export function getGarden(id: string): Promise<Garden> {
  return apiFetch<Garden>(`/api/gardens/${id}`)
}

export function createGarden(request: CreateGardenRequest): Promise<Garden> {
  return apiFetch<Garden>('/api/gardens', { method: 'POST', body: JSON.stringify(request) })
}

export function updateGarden(id: string, request: UpdateGardenRequest): Promise<Garden> {
  return apiFetch<Garden>(`/api/gardens/${id}`, { method: 'PUT', body: JSON.stringify(request) })
}

export function deleteGarden(id: string): Promise<void> {
  return apiFetch<void>(`/api/gardens/${id}`, { method: 'DELETE' })
}
