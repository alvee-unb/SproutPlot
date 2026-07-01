export interface CurrentWeather {
  temperatureC: number
  apparentTemperatureC?: number | null
  humidity: number
  windSpeedKmh: number
  precipitationMm: number
  weatherCode: number
  description: string
  isDay: boolean
}

export interface DailyForecast {
  date: string
  tempMinC: number
  tempMaxC: number
  precipitationSumMm: number
  precipitationProbabilityMaxPercent?: number | null
  weatherCode: number
  description: string
}

export interface WeatherResponse {
  latitude: number
  longitude: number
  timeZone?: string | null
  current: CurrentWeather
  daily: DailyForecast[]
  fetchedAtUtc: string
}

export interface GeocodeResult {
  name: string
  country?: string | null
  admin1?: string | null
  latitude: number
  longitude: number
}

export interface WeatherLocation {
  latitude: number
  longitude: number
  label: string
}
