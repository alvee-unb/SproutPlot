import { useCallback, useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import * as weatherService from '../services/weatherService'
import { ApiError } from '../services/apiClient'
import type { GeocodeResult, WeatherLocation, WeatherResponse } from '../types/weather'

const STORAGE_KEY = 'sproutplot.weatherLocation'

function loadSavedLocation(): WeatherLocation | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? (JSON.parse(raw) as WeatherLocation) : null
  } catch {
    return null
  }
}

function weatherEmoji(code: number, isDay = true): string {
  if (code === 0) return isDay ? '☀️' : '🌙'
  if (code <= 2) return '🌤️'
  if (code === 3) return '☁️'
  if (code === 45 || code === 48) return '🌫️'
  if (code >= 51 && code <= 67) return '🌧️'
  if (code >= 71 && code <= 77) return '❄️'
  if (code >= 80 && code <= 82) return '🌦️'
  if (code >= 85 && code <= 86) return '🌨️'
  if (code >= 95) return '⛈️'
  return '🌡️'
}

export function WeatherCard() {
  const [location, setLocation] = useState<WeatherLocation | null>(loadSavedLocation)
  const [weather, setWeather] = useState<WeatherResponse | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [query, setQuery] = useState('')
  const [results, setResults] = useState<GeocodeResult[]>([])
  const [searching, setSearching] = useState(false)

  const loadWeather = useCallback(async (loc: WeatherLocation) => {
    setLoading(true)
    setError(null)
    try {
      setWeather(await weatherService.getWeather(loc.latitude, loc.longitude))
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to load weather.')
    } finally {
      setLoading(false)
    }
  }, [])

  // Load weather whenever the location changes.
  useEffect(() => {
    if (location) void loadWeather(location)
  }, [location, loadWeather])

  // If we have no saved location, try the browser's geolocation once.
  useEffect(() => {
    if (location || !('geolocation' in navigator)) return
    navigator.geolocation.getCurrentPosition(
      (pos) => setLocation({ latitude: pos.coords.latitude, longitude: pos.coords.longitude, label: 'Your location' }),
      () => {
        /* denied or unavailable — user can search instead */
      },
    )
  }, [location])

  function selectLocation(loc: WeatherLocation) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(loc))
    setLocation(loc)
    setResults([])
    setQuery('')
  }

  async function handleSearch(event: FormEvent) {
    event.preventDefault()
    if (!query.trim()) return
    setSearching(true)
    setError(null)
    try {
      setResults(await weatherService.searchLocations(query.trim()))
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Search failed.')
    } finally {
      setSearching(false)
    }
  }

  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="mb-4 flex items-center justify-between gap-3">
        <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-50">Weather</h2>
        <form onSubmit={handleSearch} className="flex gap-2">
          <input
            type="search"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Search a city…"
            className="w-40 rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
          />
          <button
            type="submit"
            disabled={searching}
            className="rounded-lg bg-emerald-600 px-3 py-1.5 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:opacity-60"
          >
            {searching ? '…' : 'Search'}
          </button>
        </form>
      </div>

      {results.length > 0 && (
        <ul className="mb-4 divide-y divide-slate-100 rounded-lg border border-slate-200 dark:divide-slate-800 dark:border-slate-800">
          {results.map((r) => (
            <li key={`${r.latitude},${r.longitude}`}>
              <button
                type="button"
                onClick={() =>
                  selectLocation({
                    latitude: r.latitude,
                    longitude: r.longitude,
                    label: [r.name, r.admin1, r.country].filter(Boolean).join(', '),
                  })
                }
                className="w-full px-3 py-2 text-left text-sm text-slate-700 transition hover:bg-slate-50 dark:text-slate-200 dark:hover:bg-slate-800"
              >
                {[r.name, r.admin1, r.country].filter(Boolean).join(', ')}
              </button>
            </li>
          ))}
        </ul>
      )}

      {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}

      {!location && !error && (
        <p className="text-sm text-slate-500 dark:text-slate-400">
          Allow location access or search for a city to see local weather.
        </p>
      )}

      {loading && <p className="text-sm text-slate-500 dark:text-slate-400">Loading weather…</p>}

      {weather && !loading && (
        <div>
          <p className="mb-3 text-sm text-slate-500 dark:text-slate-400">{location?.label}</p>
          <div className="flex items-center gap-4">
            <span className="text-5xl" aria-hidden="true">
              {weatherEmoji(weather.current.weatherCode, weather.current.isDay)}
            </span>
            <div>
              <p className="text-3xl font-semibold text-slate-900 dark:text-slate-50">
                {Math.round(weather.current.temperatureC)}°C
              </p>
              <p className="text-sm text-slate-600 dark:text-slate-300">{weather.current.description}</p>
            </div>
            <div className="ml-auto text-right text-sm text-slate-500 dark:text-slate-400">
              <p>Humidity {weather.current.humidity}%</p>
              <p>Wind {Math.round(weather.current.windSpeedKmh)} km/h</p>
            </div>
          </div>

          <ul className="mt-5 grid grid-cols-5 gap-2">
            {weather.daily.slice(0, 5).map((day) => (
              <li
                key={day.date}
                className="rounded-lg bg-slate-50 p-2 text-center dark:bg-slate-800"
                title={day.description}
              >
                <p className="text-xs text-slate-500 dark:text-slate-400">
                  {new Date(day.date).toLocaleDateString(undefined, { weekday: 'short' })}
                </p>
                <p className="text-lg" aria-hidden="true">
                  {weatherEmoji(day.weatherCode)}
                </p>
                <p className="text-xs font-medium text-slate-700 dark:text-slate-200">
                  {Math.round(day.tempMaxC)}° / {Math.round(day.tempMinC)}°
                </p>
                {day.precipitationProbabilityMaxPercent != null && (
                  <p className="text-xs text-sky-600 dark:text-sky-400">💧 {day.precipitationProbabilityMaxPercent}%</p>
                )}
              </li>
            ))}
          </ul>
        </div>
      )}
    </section>
  )
}
