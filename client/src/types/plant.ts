export const PLANT_STATUSES = ['Planned', 'Growing', 'Harvested', 'Removed', 'Dead'] as const
export type PlantStatus = (typeof PLANT_STATUSES)[number]

export interface PlantType {
  id: string
  name: string
  category: 'Vegetable' | 'Fruit' | 'Herb' | 'Flower' | 'Shrub' | 'Tree' | 'Other'
}

export interface Plant {
  id: string
  gardenId: string
  plantTypeId?: string | null
  plantTypeName?: string | null
  name: string
  variety?: string | null
  datePlanted?: string | null
  quantity: number
  status: PlantStatus
  notes?: string | null
  createdAtUtc: string
  updatedAtUtc?: string | null
}

export interface PlantInput {
  name: string
  plantTypeId?: string | null
  variety?: string
  datePlanted?: string | null
  quantity: number
  status: PlantStatus
  notes?: string
}
