export interface NotificationPreferences {
  emailRemindersEnabled: boolean
  pushRemindersEnabled: boolean
  reminderLeadDays: number
}

export interface ReminderSummary {
  taskCount: number
  channelsNotified: string[]
  message: string
}
