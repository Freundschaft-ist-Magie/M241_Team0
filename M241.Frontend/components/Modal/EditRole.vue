<script lang="ts" setup>
const dialogRef = inject("dialogRef");

const originalRole = dialogRef?.value?.data?.role ?? {
  id: 0,
  name: "",
  read: false,
  write: false,
  manage: false,
};

const role = reactive({
  id: originalRole.id,
  name: originalRole.name,
  read: originalRole.read,
  write: originalRole.write,
  manage: originalRole.manage,
});

const permissions = ref([{ name: "read" }, { name: "write" }, { name: "manage" }]);

const selectedPermissions = ref([
  { name: "read", value: false },
  { name: "write", value: false },
  { name: "manage", value: false },
]);

function cancel() {
  dialogRef?.value?.close();
}

function save() {
  // apply permission changes
  selectedPermissions.value.forEach((permission) => {
    role[permission.name] = permission.value;
  });

  // save changes
  dialogRef?.value?.close({
    id: role.id,
    name: role.name,
    read: role.read,
    write: role.write,
    manage: role.manage,
  });
}
</script>

<template>
  <div class="flex items-center gap-4 mb-4">
    <label for="name" class="font-semibold w-24">Rolle</label>
    <InputText id="name" class="flex-auto" autocomplete="off" v-model="role.name" />
  </div>
  <div class="flex items-center gap-4 mb-8">
    <label for="permissions" class="font-semibold w-24">Berechtigungen</label>
    <MultiSelect
      v-model="selectedPermissions"
      display="chip"
      :options="selectedPermissions"
      optionLabel="name"
      filter
      placeholder="Berechtigungen auswählen"
      class="w-full"
    />
  </div>
  <div>
    <div class="flex justify-end gap-2">
      <Button
        type="button"
        label="Abbrechen"
        severity="secondary"
        @click="cancel"
      ></Button>
      <Button type="button" label="Speichern" @click="save"></Button>
    </div>
  </div>
</template>
