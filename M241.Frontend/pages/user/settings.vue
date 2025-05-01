<script lang="ts" setup>
import { useToastStore } from "@/utils/stores/base/ToastStore";

const userData = ref({
  id: "Dh42-das2-8pw6-Qn7C",
  username: "Max Mustermann",
  email: "max.mustermann@muster.com",
  role: "Benutzer",
  assignedDevices: "Arduino 1",
  lastLogin: "2024-12-24",
  status: "Aktiv",
  notes: "ist Teil der BM & normalen Klasse",
  notifications: true,
});

const editableUserData = ref({ ...userData.value });

const newPassword = ref({
  password: "",
  passwordConfirm: "",
});

function saveChanges() {
  if (newPassword.value.password !== newPassword.value.passwordConfirm) {
    console.error("Passwords do not match!");
    useToastStore().setToast("error", "Fehler", "Die Passwörter stimmen nicht überein.");
    return;
  }

  userData.value = { ...editableUserData.value };
  // Handle password update logic here if newPassword.password is not empty
  console.log("Changes saved:", userData.value);
  newPassword.value.password = "";
  newPassword.value.passwordConfirm = "";
}
</script>

<template>
  <div
    class="w-full md:w-3/4 lg:w-2/3 xl:w-1/2 mx-auto flex flex-col gap-4 px-4 sm:px-6 py-6"
  >
    <div class="mt-6 border-b border-gray-200 pb-4 mb-4">
      <div class="flex items-center gap-4">
        <div class="w-16 h-16 bg-gray-800 rounded-full flex-shrink-0"></div>
        <div class="min-w-0">
          <h2 class="text-lg font-semibold text-gray-900 truncate">
            {{ editableUserData.username }}
          </h2>
          <p class="text-gray-500 truncate">{{ editableUserData.email }}</p>
        </div>
      </div>
    </div>

    <div class="flex flex-col gap-8 md:flex-row md:justify-between md:gap-8 lg:gap-12">
      <div class="flex flex-col gap-4 w-full md:flex-1 md:max-w-md lg:max-w-lg">
        <div class="flex flex-col gap-1.5">
          <label for="username_label" class="text-sm font-medium text-gray-700"
            >Benutzername</label
          >
          <InputText
            id="username_label"
            v-model="editableUserData.username"
            class="w-full"
            aria-describedby="username-help"
          />
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="email_label" class="text-sm font-medium text-gray-700"
            >E-Mail</label
          >
          <InputText
            type="email"
            id="email_label"
            v-model="editableUserData.email"
            class="w-full bg-gray-100"
            disabled
          />
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="password_label" class="text-sm font-medium text-gray-700"
            >Neues Passwort</label
          >
          <InputText
            id="password_label"
            type="password"
            v-model="newPassword.password"
            class="w-full"
            placeholder="Mindestens 8 Zeichen"
          />
        </div>

        <div class="flex flex-col gap-1.5">
          <label for="confirm_label" class="text-sm font-medium text-gray-700"
            >Passwort bestätigen</label
          >
          <InputText
            id="confirm_label"
            type="password"
            v-model="newPassword.passwordConfirm"
            class="w-full"
            placeholder="Passwort erneut eingeben"
            :invalid="
              newPassword.password !== '' &&
              newPassword.password !== newPassword.passwordConfirm
            "
          />
          <small
            v-if="
              newPassword.password !== '' &&
              newPassword.password !== newPassword.passwordConfirm
            "
            class="text-red-600"
          >
            Passwörter stimmen nicht überein.
          </small>
        </div>

        <Button
          label="Speichern"
          icon="pi pi-save"
          severity="info"
          class="text-white! bg-primary1! hover:bg-primary1/90! active:bg-primary2! w-full md:w-auto md:self-start mt-2"
          @click="saveChanges"
          :disabled="
            newPassword.password !== newPassword.passwordConfirm &&
            newPassword.passwordConfirm !== ''
          "
        />
      </div>

      <div
        class="w-full md:w-auto md:basis-1/3 lg:basis-2/5 border-t border-gray-200 pt-6 md:border-t-0 md:pt-0 md:border-l md:pl-8 lg:pl-12"
      >
        <h3 class="text-base font-semibold text-gray-900">Allgemeine Informationen</h3>
        <ul class="text-sm text-gray-600 mt-3 space-y-2">
          <li>
            <strong class="font-medium text-gray-800">Id:</strong>
            <span class="break-all">{{ userData.id }}</span>
          </li>
          <li>
            <strong class="font-medium text-gray-800">Rolle:</strong> {{ userData.role }}
          </li>
          <li>
            <strong class="font-medium text-gray-800">Zugewiesene Geräte:</strong>
            {{ userData.assignedDevices }}
          </li>
          <li>
            <strong class="font-medium text-gray-800">Letzte Anmeldung:</strong>
            {{ userData.lastLogin }}
          </li>
          <li>
            <strong class="font-medium text-gray-800">Status:</strong>
            {{ userData.status }}
          </li>
          <li>
            <strong class="font-medium text-gray-800">Notizen:</strong>
            {{ userData.notes }}
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>
