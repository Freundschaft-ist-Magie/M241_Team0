<script setup lang="ts">
import { useUserStore } from "~/utils/stores/UserStore";
import { useConfirmStore } from "~/utils/stores/base/ConfirmStore";
import EditUser from "./Modal/EditUser.vue";

const dialog = useDialog();

const props = defineProps<{
  users: {
    id: number;
    name: string;
    email: string;
    roles: { id: number; name: string; permissions: string }[];
  }[];
  roles: {
    id: number;
    name: string;
    permissions: string;
  }[];
}>();

function addUser() {
  editUser({
    id: 0,
    name: "",
    email: "",
    roles: [],
  });
}

function editUser(user: {
  id: number;
  name: string;
  email: string;
  roles: { id: number; name: string; permissions: string }[];
}) {
  dialog.open(EditUser, {
    props: {
      header: `${user.name || "Neuer Benutzer"} bearbeiten`,
      style: { width: "50vw" },
      modal: true,
    },
    data: {
      user: user,
      roles: props.roles,
    },
    onClose: (options) => {
      const result = options?.data;
      if (result) {
        useUserStore().UpdateUser(result);
      } else {
        console.log("Abgebrochen oder geschlossen");
      }
    },
  });
}

function deleteUser(user: {
  id: number;
  name: string;
  email: string;
  roles: { id: number; name: string; permissions: string }[];
}) {
  useConfirmStore().setConfirm(
    `Bestätigen Sie, dass sie ${user.name} löschen wollten?`,
    "Löschen bestätigen",
    "pi pi-exclamation-triangle",
    () => {
      useUserStore().Delete(user);
    },
    () => {},
    "Löschen",
    "Abbrechen",
    "danger"
  );
}
</script>

<template>
  <DataTable :value="users" tableStyle="min-width: 50rem">
    <template #header>
      <span class="flex justify-end">
        <Button text icon="" @click="addUser" rounded>
          <Icon class="text-2xl" name="mdi-light:plus" />
        </Button>
      </span>
    </template>
    <Column field="id" header="ID"></Column>
    <Column field="name" header="User Name"></Column>
    <Column field="email" header="Email"></Column>
    <Column header="Roles">
      <template #body="{ data }">
        <div v-if="data.roles && data.roles.length > 0">
          <span v-for="(role, index) in data.roles" :key="role.id">
            {{ role.name }}<span v-if="index < data.roles.length - 1">, </span>
          </span>
        </div>
        <div v-else>
          <span class="text-surface-500 italic">Keine Rolle</span>
        </div>
      </template>
    </Column>
    <Column class="w-24 !text-end">
      <template #body="{ data }">
        <div class="flex items-center justify-between gap-2">
          <Button
            @click="editUser(data)"
            severity="info"
            rounded
            class="bg-primary1! hover:bg-primary2/80! active:bg-primary2!"
          >
            <Icon name="mdi:pencil" />
          </Button>
          <Button icon="" @click="deleteUser(data)" severity="danger" rounded>
            <Icon name="mdi:delete" />
          </Button>
        </div>
      </template>
    </Column>
  </DataTable>
  <Toast />
</template>

<style scoped></style>
