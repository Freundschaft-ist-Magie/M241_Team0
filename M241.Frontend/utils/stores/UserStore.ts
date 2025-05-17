import { defineStore } from "pinia";
import { get, post, put } from "~/utils/services/base/ApiService";
import { useToastStore } from "./base/ToastStore";
import User from "~/models/User";

export const useUserStore = defineStore("user", () => {
  const _userBaseUrl = "Users";
  const _users = ref<User[]>([]);

  const Users = computed(() => _users.value);

  async function GetAll() {
    const users: User[] = await get(_userBaseUrl);

    return users;
  }

  async function Create(user: User) {
    try {
      user.id = _users.value.length + 1;
      _users.value.push(user);

      useToastStore().setToast(
        "success",
        "Hinzugefügt",
        "Benutzer erfolgreich hinzugefügt."
      );
    } catch (e) {
      console.error("Fehler beim Erstellen eines neuen Benutzers:", e);

      useToastStore().setToast(
        "error",
        "Fehler",
        "Benutzer konnte nicht hinzugefügt werden."
      );
    }
  }

  async function UpdateUser(user: {
    id: number;
    name: string;
    email: string;
    roles: { id: number; name: string; permissions: string }[];
  }) {
    const index = users.value.findIndex((u) => u.id === user.id);

    if (user.id === 0) {
      await Create(user);
      return;
    }

    if (index !== -1) {
      users.value[index] = { ...users.value[index], ...user };

      useToastStore().setToast(
        "success",
        "Aktualisiert",
        "Benutzer erfolgreich aktualisiert."
      );
      return users.value[index];
    } else {
      console.error("Konnte Benutzer mit ID nicht aktualisieren:", user.id);

      useToastStore().setToast(
        "error",
        "Fehler",
        "Benutzer konnte nicht aktualisiert werden."
      );
      return null;
    }
  }

  async function Delete(user: { id: number; name: string; email: string; roles: any[] }) {
    const index = users.value.findIndex((u) => u.id === user.id);

    if (index !== -1) {
      const removed = users.value.splice(index, 1)[0];

      useToastStore().setToast("success", "Entfernt", "Benutzer erfolgreich entfernt.");

      return removed;
    } else {
      console.error("Benutzer mit ID nicht gefunden:", user.id);

      useToastStore().setToast(
        "error",
        "Fehler",
        "Benutzer konnte nicht gelöscht werden."
      );

      return null;
    }
  }

  return { Users, GetAll, Create, UpdateUser, Delete };
});
