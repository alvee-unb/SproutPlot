import { apiFetch } from './apiClient'
import type { GeocodeResult, WeatherResponse } from '../types/weather'

export function getWeather(latitude: number, longitude: number): Promise<WeatherResponse> {
  return apiFetch<WeatherResponse>(`/api/weather?latitude=${latitude}&longitude=${longitude}`)
}

export function searchLocations(name: string): Promise<GeocodeResult[]> {
  return apiFetch<GeocodeResult[]>(`/api/weather/search?name=${encodeURIComponent(name)}`)
}
