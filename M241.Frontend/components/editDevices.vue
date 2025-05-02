<script setup lang="ts">
import { ref, onMounted } from "vue";
import Room from "~/models/RoomData";
import { useRoomStore } from "~/utils/stores/RoomStore";

const rooms = ref<Room[]>([]);
const roomStore = useRoomStore();

const confirm = useConfirm();
const toast = useToast();

// ---- Room Edit dialog ----
const showEdit = ref(false);
const editData = ref({
  id: 0,
  name: "",
  room: "",
  assigned_user: "",
});

// const selectRow = (data: Object) => {
//   toast.add({
//     severity: "info",
//     summary: data.name,
//     detail: data.assigned_user,
//     life: 3000,
//   });
// };

// const addEntry = async (data: Object) => {
//   const newRoom: Room = {
//     name: "New Device",
//     macAddress: "00:00:00:00:00",
//     isBurning: false,
//   };

//   await RoomService.AddRoom(newRoom);

//   toast.add({
//     severity: "success",
//     summary: "Hinzugefügt",
//     detail: "Ein neues Gerät wurde hinzugefügt",
//     life: 3000,
//   });
// };

const prefill = (data: Object) => {
  editData.value.name = data.name;
  editData.value.room = data.room;
  editData.value.assigned_user = data.assigned_user;
  editData.value.id = data.id;

  showEdit.value = true;
};

const editEntry = async () => {
  const updatedRoom = editData.value;
  const index = rooms.value.findIndex((r) => r.id === updatedRoom.id);

  if (index !== -1) {
    rooms.value[index] = { ...rooms.value[index], ...updatedRoom };
    await roomStore.UpdateRoom(updatedRoom);
    toast.add({
      severity: "success",
      summary: "Edited",
      detail: "Das Gerät wurde geändert.",
      life: 3000,
    });
  } else {
    toast.add({
      severity: "warn",
      summary: "Edited",
      detail: "Das Gerät konnte nicht gefunden werden.",
      life: 3000,
    });
  }

  showEdit.value = false;
};

// const deleteEntry = (data: Object) => {
//   confirm.require({
//     message: 'Do you want to delete "' + data.name + '" ?',
//     header: "Danger Zone",
//     icon: "pi pi-info-circle",
//     rejectLabel: "Cancel",
//     rejectProps: {
//       label: "Cancel",
//       severity: "secondary",
//       outlined: true,
//     },
//     acceptProps: {
//       label: "Delete",
//       severity: "danger",
//     },
//     accept: () => {
//       toast.add({
//         severity: "info",
//         summary: "Confirmed",
//         detail: "Record deleted",
//         life: 3000,
//       });

//       const index = rooms.value.findIndex((room) => room.id === data.id);
//       if (index !== -1) {
//         rooms.value.splice(index, 1);
//       }
//     },
//     reject: () => {
//       toast.add({
//         severity: "error",
//         summary: "Rejected",
//         detail: "You have rejected",
//         life: 3000,
//       });
//     },
//   });
// };

onMounted(async () => {
  rooms.value = await roomStore.GetAll();
  rooms.value.sort((a, b) => a.id - b.id);
});
</script>

<template>
  <DataTable :value="rooms" tableStyle="min-width: 50rem">
    <!-- <template #header>
      <span class="flex justify-end">
        <Button text icon="" @click="addEntry" rounded>
          <Icon class="text-2xl" name="mdi-light:plus" />
        </Button>
      </span>
    </template> -->

    <Column field="id" header="ID"></Column>
    <Column field="name" header="Gerätename"></Column>
    <Column field="macAddress" header="Mac-Adresse"></Column>

    <Column class="w-24 !text-end">
      <template #body="{ data }">
        <div class="flex items-center justify-between gap-2">
          <Button icon="" @click="prefill(data)" severity="secondary" rounded>
            <Icon name="mdi-light:pencil" />
          </Button>
          <!-- <Button icon="" @click="deleteEntry(data)" severity="danger" rounded>
            <Icon name="mdi-light:delete" />
          </Button> -->
        </div>
      </template>
    </Column>
  </DataTable>
  <Toast />

  <Dialog
    v-model:visible="showEdit"
    modal
    header="Edit Profile"
    :style="{ width: '25rem' }"
  >
    <span class="text-surface-500 dark:text-surface-400 block mb-8"
      >Update your information.</span
    >
    <div class="flex items-center gap-4 mb-4">
      <label for="name" class="font-semibold w-24">Device Name</label>
      <InputText id="name" class="flex-auto" autocomplete="off" v-model="editData.name" />
    </div>
    <div class="flex justify-end gap-2">
      <Button
        type="button"
        label="Cancel"
        severity="secondary"
        @click="showEdit = false"
      ></Button>
      <Button type="button" label="Save" @click="editEntry()"></Button>
    </div>
  </Dialog>
</template>

<style scoped></style>
