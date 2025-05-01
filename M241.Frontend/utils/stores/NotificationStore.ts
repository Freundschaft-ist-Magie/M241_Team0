import { defineStore } from 'pinia';
import Notification from '~/models/Notification';
import { useToastStore } from './base/ToastStore';

export const useNotificationStore = defineStore("notification", () => {
  const _notifications = ref([
    new Notification(1, "User Jane wurde erstellt", new Date(Date.now() - 9 * 60 * 60 * 1000).toISOString(), false, "https://placeholder.pics/svg/50"),
    new Notification(2, "Neuer Sensor installiert", new Date(Date.now() - 25 * 60 * 60 * 1000).toISOString(), false, null),
    new Notification(3, "Wartung abgeschlossen", new Date(Date.now() - 3 * 24 * 60 * 60 * 1000).toISOString(), true, "https://placeholder.pics/svg/50/ff0000"),
    new Notification(4, "System Update verfügbar", new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString(), false, null),
    new Notification(5, "Monatsreport generiert", new Date(Date.now() - 40 * 24 * 60 * 60 * 1000).toISOString(), true, "https://placeholder.pics/svg/50/00ff00"),
  ])
  const notifications = computed(() => _notifications);

  async function GetAll() {
    return _notifications.value;
  }

  async function Create(notification: Notification) {
    try {
      notification.id = _notifications.value.length + 1;
      _notifications.value.push(notification);
    } catch (e) {
      console.error("Error, during creating a new Notification");
    }
  }

  async function UpdateRead(notification: Notification, isRead: boolean) {
    const index = _notifications.value.findIndex(n => n.id === notification.id);

    if (index !== -1) {
      notification.read = isRead;

      _notifications.value[index] = { ..._notifications.value[index], ...notification };

      useToastStore().setToast(
        "success",
        "Aktualisiert",
        `Benachrichtigung als ${isRead ? "ungelesen" : "gelesen"} markiert`
      );
      return _notifications.value[index];
    } else {
      console.error("Was not able to update Notification with ID:", notification.id);

      useToastStore().setToast(
        "error",
        "Fehler",
        "Benachrichtigung konnte nicht aktuallisiert werden"
      );
      return null;
    }
  }

  async function UpdateMultipleRead(notifications: Notification[], isRead: boolean) {
    let hasError = false;

    notifications.forEach(notification => {
      const index = _notifications.value.findIndex(n => n.id === notification.id);

      if (index !== -1) {
        notification.read = isRead;

        _notifications.value[index] = { ..._notifications.value[index], ...notification };
      } else {
        console.error("Was not able to update Notification with ID:", notification.id);
        hasError = true;
      }
    });

    if (hasError) {
      useToastStore().setToast(
        "error",
        "Fehler",
        "Benachrichtigungen konnten nicht aktualisiert werden"
      );
    } else {
      useToastStore().setToast(
        "success",
        "Aktualisiert",
        `Benachrichtigungen als ${isRead ? "ungelesen" : "gelesen"} markiert`
      );
    }
  }

  async function Delete(notification: Notification) {
    const index = _notifications.value.findIndex(n => n.id === notification.id);

    if (index !== -1) {
      const removed = _notifications.value.splice(index, 1)[0];

      useToastStore().setToast(
        "success",
        "Entfernt",
        "Benachrichtigung entfernt"
      );
      return removed;
    } else {
      console.error("Was not able to update Notification with ID:", notification.id);

      useToastStore().setToast(
        "error",
        "Fehler",
        "Benachrichtigung konnte nicht entfernt werden"
      );
      return null;
    }
  }

  async function DeleteMultiple(notifications: Notification[]) {
    let hasError = false;

    notifications.forEach(notification => {
      const index = _notifications.value.findIndex(n => n.id === notification.id);

      if (index !== -1) {
        _notifications.value.splice(index, 1);
      } else {
        console.error("Was not able to delete Notification with ID:", notification.id);
        hasError = true;
      }
    });

    if (hasError) {
      useToastStore().setToast(
        "error",
        "Fehler",
        "Benachrichtigungen konnten nicht entfernt werden"
      );
    } else {
      useToastStore().setToast(
        "success",
        "Entfernt",
        "Benachrichtigungen entfernt"
      );
    }
  }

  return { notifications, GetAll, Create, UpdateRead, UpdateMultipleRead, Delete, DeleteMultiple };
});