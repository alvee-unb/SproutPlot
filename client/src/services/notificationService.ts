import { apiFetch } from './apiClient'
import type { NotificationPreferences, ReminderSummary } from '../types/notifications'

export function getPreferences(): Promise<NotificationPreferences> {
  return apiFetch<NotificationPreferences>('/api/notifications/preferences')
}

export function updatePreferences(prefs: NotificationPreferences): Promise<NotificationPreferences> {
  return apiFetch<NotificationPreferences>('/api/notifications/preferences', {
    method: 'PUT',
    body: JSON.stringify(prefs),
  })
}

export function sendReminders(): Promise<ReminderSummary> {
  return apiFetch<ReminderSummary>('/api/notifications/send-reminders', { method: 'POST' })
}
