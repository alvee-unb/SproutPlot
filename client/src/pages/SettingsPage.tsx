import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import * as notificationService from '../services/notificationService'
import { ApiError } from '../services/apiClient'
import type { NotificationPreferences, ReminderSummary } from '../types/notifications'

export function SettingsPage() {
  const [prefs, setPrefs] = useState<NotificationPreferences | null>(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [sending, setSending] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [saved, setSaved] = useState(false)
  const [reminder, setReminder] = useState<ReminderSummary | null>(null)

  useEffect(() => {
    notificationService
      .getPreferences()
      .then(setPrefs)
      .catch((err) => setError(err instanceof ApiError ? err.message : 'Failed to load settings.'))
      .finally(() => setLoading(false))
  }, [])

  async function handleSave(event: FormEvent) {
    event.preventDefault()
    if (!prefs) return
    setSaving(true)
    setSaved(false)
    setError(null)
    try {
      setPrefs(await notificationService.updatePreferences(prefs))
      setSaved(true)
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to save settings.')
    } finally {
      setSaving(false)
    }
  }

  async function handleSendNow() {
    setSending(true)
    setReminder(null)
    setError(null)
    try {
      setReminder(await notificationService.sendReminders())
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Failed to send reminder.')
    } finally {
      setSending(false)
    }
  }

  if (loading) return <p className="text-slate-500 dark:text-slate-400">Loading…</p>
  if (!prefs) return <p className="text-red-600 dark:text-red-400">{error ?? 'Settings unavailable.'}</p>

  return (
    <div className="max-w-xl space-y-6">
      <h1 className="text-2xl font-semibold text-slate-900 dark:text-slate-50">Settings</h1>

      <form
        onSubmit={handleSave}
        className="space-y-4 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-800 dark:bg-slate-900"
      >
        <h2 className="text-sm font-semibold text-slate-700 dark:text-slate-300">Task reminders</h2>

        <label className="flex items-center gap-3">
          <input
            type="checkbox"
            checked={prefs.emailRemindersEnabled}
            onChange={(e) => setPrefs({ ...prefs, emailRemindersEnabled: e.target.checked })}
            className="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500"
          />
          <span className="text-sm text-slate-700 dark:text-slate-200">Email reminders</span>
        </label>

        <label className="flex items-center gap-3">
          <input
            type="checkbox"
            checked={prefs.pushRemindersEnabled}
            onChange={(e) => setPrefs({ ...prefs, pushRemindersEnabled: e.target.checked })}
            className="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500"
          />
          <span className="text-sm text-slate-700 dark:text-slate-200">Push reminders</span>
        </label>

        <label className="block">
          <span className="mb-1 block text-sm text-slate-700 dark:text-slate-200">
            Remind me about tasks due within (days)
          </span>
          <input
            type="number"
            min={0}
            max={30}
            value={prefs.reminderLeadDays}
            onChange={(e) => setPrefs({ ...prefs, reminderLeadDays: Number(e.target.value) })}
            className="w-24 rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
          />
        </label>

        {error && <p className="text-sm text-red-600 dark:text-red-400">{error}</p>}
        {saved && <p className="text-sm text-emerald-600 dark:text-emerald-400">Saved.</p>}

        <div className="flex items-center gap-3">
          <button
            type="submit"
            disabled={saving}
            className="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:opacity-60"
          >
            {saving ? 'Saving…' : 'Save settings'}
          </button>
          <button
            type="button"
            onClick={handleSendNow}
            disabled={sending}
            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-100 disabled:opacity-60 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
          >
            {sending ? 'Sending…' : 'Send me a reminder now'}
          </button>
        </div>

        {reminder && <p className="text-sm text-slate-600 dark:text-slate-300">{reminder.message}</p>}
      </form>

      <p className="text-xs text-slate-400">
        Reminders currently log on the server; email and push delivery plug in behind the same interface later.
      </p>
    </div>
  )
}
