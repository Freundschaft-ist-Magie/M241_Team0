import {
  formatDistanceToNow,
  parseISO,
  isToday,
  isYesterday,
  isThisWeek,
  isThisMonth,
} from "date-fns";
import { de } from "date-fns/locale";

export function formatRelativeTime(isoTimestamp: string): string {
  try {
    const date = parseISO(isoTimestamp);
    return formatDistanceToNow(date, { addSuffix: true, locale: de });
  } catch (e) {
    console.error("Error parsing date:", isoTimestamp, e);
    return "Ungültiges Datum";
  }
}

export function getNotificationGroup(isoTimestamp: string): string {
  try {
    const date = parseISO(isoTimestamp);
    if (isToday(date)) return "Heute";
    if (isYesterday(date)) return "Gestern";
    if (isThisWeek(date, { locale: de, weekStartsOn: 1 })) return "Diese Woche";
    if (isThisMonth(date)) return "Diesen Monat";
    return "Älter";
  } catch (e) {
    console.error("Error getting group for date:", isoTimestamp, e);
    return "Unbekannt";
  }
}