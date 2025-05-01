<script setup lang="ts">
import { useDialog } from "#imports";
import { useConfirmStore } from "~/utils/stores/base/ConfirmStore";
import { useRoleStore } from "~/utils/stores/RoleStore";
import EditRole from "./Modal/EditRole.vue";

defineProps<{
  roles: {
    id: number;
    name: string;
    permissions: string;
  }[];
}>();

const permissions = [
  { name: "read", label: "Lesen" },
  { name: "write", label: "Schreiben" },
  { name: "manage", label: "Verwalten" },
];

const dialog = useDialog();

const addRole = () => {
  editRole({
    id: 0, // id 0 will trigger 'create' inside UpdateRole
    name: "",
    permissions: "",
  });
};

function editRole(role: { id: number; name: string; permissions: string }) {
  dialog.open(EditRole, {
    props: {
      header: `${role.name} bearbeiten`,
      style: { width: "50vw" },
      modal: true,
    },
    data: {
      role: role,
    },
    onClose: (options) => {
      const result = options?.data;
      if (result) {
        useRoleStore().UpdateRole(result);
      } else {
        console.log("Abgebrochen oder geschlossen");
      }
    },
  });
}

const deleteRole = (role: { id: number; name: string; permissions: string }) => {
  useConfirmStore().setConfirm(
    `Bestätigen Sie, dass sie ${role.name} löschen wollten?`,
    "Löschen bestätigen",
    "pi pi-exclamation-triangle",
    () => {
      useRoleStore().Delete(role);
    },
    () => {},
    "Löschen",
    "Abbrechen",
    "danger"
  );
};
</script>

<template>
  <DataTable :value="roles" tableStyle="min-width: 50rem">
    <template #header>
      <div class="flex justify-end">
        <Button text icon="" @click="addRole" rounded>
          <Icon class="text-2xl" name="mdi:plus" />
        </Button>
      </div>
    </template>
    <Column field="id" header="ID"></Column>
    <Column field="name" header="Rolle"></Column>
    <!--
    <Column field="users" header="Anz. Benutzer"></Column>
    -->
    <Column header="Berechtigungen">
      <template #body="{ data }">
        <div class="flex items-center gap-2">
          <span class="mr-2" v-for="p in permissions" :key="p.name">
            <Checkbox v-model="data[p.name]" disabled binary />
            {{ p.label }}
          </span>
        </div>
      </template>
    </Column>

    <Column>
      <template #body="{ data }">
        <div class="flex items-center justify-end gap-2">
          <Button
            @click="editRole(data)"
            severity="info"
            rounded
            class="bg-primary1! hover:bg-primary2/80! active:bg-primary2!"
          >
            <Icon name="mdi:pencil" />
          </Button>
          <Button @click="deleteRole(data)" severity="danger" rounded>
            <Icon name="mdi:delete" />
          </Button>
        </div>
      </template>
    </Column>
  </DataTable>
</template>
