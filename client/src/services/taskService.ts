import { apiFetch } from './apiClient'
import type { PagedResult } from '../types/garden'
import type { CreateTaskRequest, GardenTask, TaskListParams, UpdateTaskRequest } from '../types/task'

function query(params: TaskListParams): string {
  const search = new URLSearchParams()
  if (params.status) search.set('status', params.status)
  if (params.dueOnOrBefore) search.set('dueOnOrBefore', params.dueOnOrBefore)
  const qs = search.toString()
  return qs ? `?${qs}` : ''
}

export function listTasks(params: TaskListParams = {}): Promise<PagedResult<GardenTask>> {
  return apiFetch<PagedResult<GardenTask>>(`/api/tasks${query(params)}`)
}

export function listGardenTasks(gardenId: string, params: TaskListParams = {}): Promise<PagedResult<GardenTask>> {
  return apiFetch<PagedResult<GardenTask>>(`/api/gardens/${gardenId}/tasks${query(params)}`)
}

export function createTask(gardenId: string, request: CreateTaskRequest): Promise<GardenTask> {
  return apiFetch<GardenTask>(`/api/gardens/${gardenId}/tasks`, { method: 'POST', body: JSON.stringify(request) })
}

export function updateTask(id: string, request: UpdateTaskRequest): Promise<GardenTask> {
  return apiFetch<GardenTask>(`/api/tasks/${id}`, { method: 'PUT', body: JSON.stringify(request) })
}

export function completeTask(id: string): Promise<GardenTask> {
  return apiFetch<GardenTask>(`/api/tasks/${id}/complete`, { method: 'POST' })
}

export function snoozeTask(id: string, days: number): Promise<GardenTask> {
  return apiFetch<GardenTask>(`/api/tasks/${id}/snooze`, { method: 'POST', body: JSON.stringify({ days }) })
}

export function deleteTask(id: string): Promise<void> {
  return apiFetch<void>(`/api/tasks/${id}`, { method: 'DELETE' })
}
