<script lang="ts" setup>
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

const newPassword = ref({
  password: "",
  passwordConfirm: "",
});

const checkboxValue = ref(false);
</script>

<template>
  <div class="flex flex-col gap-4">
    <div class="mt-6 border-b border-gray2 my-4">
      <div class="flex items-center gap-4 mb-4">
        <div class="w-16 h-16 bg-black rounded-full"></div>
        <div>
          <h2 class="text-lg font-semibold text-black">{{ userData.username }}</h2>
          <p class="text-gray1">{{ userData.email }}</p>
        </div>
      </div>
    </div>

    <div class="flex justify-between gap-16">
      <div class="flex flex-col gap-8">
        <FloatLabel class="w-96!">
          <InputText id="over_label" v-model="userData.username" class="w-full" />
          <label for="over_label">Benutzername</label>
        </FloatLabel>

        <FloatLabel class="w-96!">
          <InputText id="over_label" v-model="userData.email" class="w-full" />
          <label for="over_label">E-Mail</label>
        </FloatLabel>

        <FloatLabel class="w-96!">
          <InputText
            id="over_label"
            type="password"
            v-model="newPassword.password"
            class="w-full"
          />
          <label for="over_label">Neues Passwort</label>
        </FloatLabel>

        <FloatLabel class="w-96!">
          <InputText
            id="over_label"
            type="password"
            v-model="newPassword.passwordConfirm"
            class="w-full"
          />
          <label for="over_label">Neues Passwort</label>
        </FloatLabel>

        <Button
          label="Speichern"
          icon="pi pi-save"
          severity="info"
          class="text-white! bg-primary1! hover:bg-primary2/80! active:bg-primary2!"
        />
      </div>

      <div>
        <h3 class="text font-semibold text-black">Allgemeine Informationen</h3>
        <ul class="text text-gray-600 mt-2">
          <li><strong>Id:</strong> Dh42-das2-8pw6-Qn7C</li>
          <li><strong>Rolle:</strong> Benutzer</li>
          <li><strong>Zugewiesene Geräte:</strong> Arduino 1</li>
          <li><strong>Letzte Anmeldung:</strong> 2024-12-24</li>
          <li><strong>Status:</strong> Aktiv</li>
          <li><strong>Notizen:</strong> ist Teil der BM & normalen Klasse</li>
        </ul>
      </div>
    </div>

    <div class="mt-4 flex items-center">
      <div class="flex items-center gap-2">
        <Checkbox
          v-model="checkboxValue"
          inputId="checkbox"
          value="Benachrichtigungen erhalten"
        />
        <label for="checkbox"> Benachrichtigungen erhalten </label>
      </div>
    </div>
  </div>
</template>
