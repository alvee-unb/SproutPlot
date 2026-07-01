export const TASK_TYPES = [
  'Water',
  'Fertilize',
  'Weed',
  'Mulch',
  'Harvest',
  'Prune',
  'Trim',
  'Repot',
  'PestInspection',
  'DiseaseInspection',
  'Other',
] as const
export type TaskType = (typeof TASK_TYPES)[number]

export type TaskStatus = 'Pending' | 'Completed'

export interface GardenTask {
  id: string
  gardenId: string
  gardenName?: string | null
  plantId?: string | null
  plantName?: string | null
  type: TaskType
  title?: string | null
  dueOn: string
  status: TaskStatus
  completedAtUtc?: string | null
  notes?: string | null
  createdAtUtc: string
}

export interface CreateTaskRequest {
  type: TaskType
  title?: string
  plantId?: string | null
  dueOn: string
  notes?: string
}

export interface UpdateTaskRequest {
  type: TaskType
  title?: string
  dueOn: string
  notes?: string
}

export interface TaskListParams {
  status?: TaskStatus
  dueOnOrBefore?: string
}

/** Human label for a task type (splits PascalCase, e.g. PestInspection → "Pest inspection"). */
export function taskTypeLabel(type: TaskType): string {
  const spaced = type.replace(/([a-z])([A-Z])/g, '$1 $2')
  return spaced.charAt(0).toUpperCase() + spaced.slice(1).toLowerCase()
}
