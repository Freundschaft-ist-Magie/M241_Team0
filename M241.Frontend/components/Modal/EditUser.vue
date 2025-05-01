<script lang="ts" setup>
const dialogRef = inject("dialogRef");

const originalUser = dialogRef?.value?.data?.user ?? {
  id: 0,
  name: "",
  email: "",
  roles: [],
};

const user = reactive({
  id: originalUser.id,
  name: originalUser.name,
  email: originalUser.email,
  roles: originalUser.roles,
});

function cancel() {
  dialogRef?.value?.close();
}

function save() {
  dialogRef?.value?.close({
    id: user.id,
    name: user.name,
    email: user.email,
    roles: user.roles,
  });
}
</script>

<template>
  <div class="flex items-center gap-4 mb-4">
    <label for="name" class="font-semibold w-24">Name</label>
    <InputText id="name" class="flex-auto" autocomplete="off" v-model="user.name" />
  </div>
  <div class="flex items-center gap-4 mb-4">
    <label for="email" class="font-semibold w-24">Email</label>
    <InputText id="email" class="flex-auto" autocomplete="off" v-model="user.email" />
  </div>
  <div class="flex items-center gap-4 mb-8">
    <label for="roles" class="font-semibold w-24">Rollen</label>
    <MultiSelect
      id="roles"
      v-model="user.roles"
      display="chip"
      :options="dialogRef?.value?.data?.roles"
      optionLabel="name"
      filter
      placeholder="Rollen auswählen"
      :maxSelectedLabels="3"
      class="w-full"
    />
  </div>
  <div class="flex justify-end gap-2">
    <Button type="button" label="Abbrechen" severity="secondary" @click="cancel"></Button>
    <Button type="button" label="Speichern" @click="save"></Button>
  </div>
</template>
