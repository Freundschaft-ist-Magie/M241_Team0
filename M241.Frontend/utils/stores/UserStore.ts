import { defineStore } from "pinia";
import { useToastStore } from "./base/ToastStore";

export const useUserStore = defineStore("user", () => {
  const users = ref([
    {
      id: 1,
      name: "Benutzer",
      email: "benutzer@example.com",
      roles: [
        {
          id: 1,
          name: "Leser",
          permissions: "Lesen",
        },
      ],
    },
    {
      id: 2,
      name: "Admin",
      email: "admin@example.com",
      roles: [
        {
          id: 2,
          name: "Administrator",
          permissions: "Administrator",
        },
      ],
    },
  ]);

  const Users = computed(() => users.value);

  async function GetAll() {
    return users.value;
  }

  async function Create(user: {
    id: number;
    name: string;
    email: string;
    roles: { id: number; name: string; permissions: string }[];
  }) {
    try {
      user.id = users.value.length + 1;
      users.value.push(user);

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
