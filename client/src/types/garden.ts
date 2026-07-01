export interface Garden {
  id: string
  name: string
  location?: string | null
  size?: string | null
  latitude?: number | null
  longitude?: number | null
  notes?: string | null
  createdAtUtc: string
  updatedAtUtc?: string | null
}

export interface CreateGardenRequest {
  name: string
  location?: string
  size?: string
  latitude?: number | null
  longitude?: number | null
  notes?: string
}

export type UpdateGardenRequest = CreateGardenRequest

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface GardenListParams {
  search?: string
  sortBy?: 'Name' | 'CreatedAt'
  descending?: boolean
  page?: number
  pageSize?: number
}
