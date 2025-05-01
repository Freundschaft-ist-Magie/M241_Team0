<script setup lang="ts">

const users = ref([{
    "id": 1,
    "name": "Device 1",
    "room": "Room 1",
    "assigned_user": "max.mustermann@mustermail.ch"
  },
  {
    "id": 2,
    "name": "Device 2",
    "room": "Room 2",
    "assigned_user": "check@tobi.de"
  }
])

const confirm = useConfirm();
const toast = useToast();

const showEdit = ref(false)
const editData = ref({
  id: 0,
  name: "",
  room: "",
  assigned_user: "",
})

const selectRow = (data: Object) => {
  toast.add({ severity: 'info', summary: data.name, detail: data.assigned_user, life: 3000 });
};

const addEntry = (data: Object) => {
  toast.add({ severity: 'success', summary: "Hinzugefügt", detail: "Ein neues Gerät wurde hinzugefügt", life: 3000 });
  users.value.push({
    name: "Added Role",
    room: "Room x",
    assigned_user: "mail@example.com",
    id: Math.floor(Math.random() * 9999)
  })
}

const prefill = (data: Object) => {
  editData.value.name = data.name
  editData.value.room = data.room
  editData.value.assigned_user = data.assigned_user
  editData.value.id = data.id

  showEdit.value = true

}

const editEntry = () => {
  const updatedUser = editData.value
  const index = users.value.findIndex(user => user.id === updatedUser.id)

  if (index !== -1) {
    users.value[index] = { ...users.value[index], ...updatedUser }
    toast.add({ severity: 'success', summary: "Edited", detail: "Das Gerät wurde geändert.", life: 3000 })
  } else {
    toast.add({ severity: 'warn', summary: "Edited", detail: "Das Gerät konnte nicht gefunden werden.", life: 3000 })
  }

  showEdit.value = false
}


const deleteEntry = (data: Object) => {
  confirm.require({
    message: 'Do you want to delete "' + data.name + '" ?',
    header: 'Danger Zone',
    icon: 'pi pi-info-circle',
    rejectLabel: 'Cancel',
    rejectProps: {
      label: 'Cancel',
      severity: 'secondary',
      outlined: true
    },
    acceptProps: {
      label: 'Delete',
      severity: 'danger'
    },
    accept: () => {
      toast.add({ severity: 'info', summary: 'Confirmed', detail: 'Record deleted', life: 3000 });

      const index = users.value.findIndex(room => room.id === data.id)
      if (index !== -1) {
        users.value.splice(index, 1)
      }

    },
    reject: () => {
      toast.add({ severity: 'error', summary: 'Rejected', detail: 'You have rejected', life: 3000 });
    }
  });
}
</script>

<template>
  <DataTable :value="users" tableStyle="min-width: 50rem">
    <template #header>
              <span class="flex justify-end">
                  <Button text icon="" @click="addEntry" rounded>
                    <Icon class="text-2xl" name="mdi-light:plus" />
                  </Button>
              </span>
    </template>
    <Column field="name" header="Device Name"></Column>
    <Column field="room" header="Room"></Column>
    <Column field="assigned_user" header="Assigned User"></Column>
    <Column class="w-24 !text-end">
      <template #body="{ data }">
        <div class="flex items-center justify-between gap-2">
          <Button icon="" @click="prefill(data)" severity="secondary" rounded>
            <Icon name="mdi-light:pencil" />
          </Button>
          <Button icon="" @click="deleteEntry(data)" severity="danger" rounded>
            <Icon name="mdi-light:delete" />
          </Button>
        </div>
      </template>
    </Column>
  </DataTable>
  <Toast />

  <Dialog v-model:visible="showEdit" modal header="Edit Profile" :style="{ width: '25rem' }">
    <span class="text-surface-500 dark:text-surface-400 block mb-8">Update your information.</span>
    <div class="flex items-center gap-4 mb-4">
      <label for="name" class="font-semibold w-24">Device Name</label>
      <InputText id="name" class="flex-auto" autocomplete="off" v-model="editData.name"  />
    </div>
    <div class="flex items-center gap-4 mb-4">
      <label for="room" class="font-semibold w-24">Room</label>
      <InputText id="room" class="flex-auto" autocomplete="off" v-model="editData.room" />
    </div>
    <div class="flex items-center gap-4 mb-12">
      <label for="assigned" class="font-semibold w-24">Assigned user</label>
      <InputText id="assigned" class="flex-auto" autocomplete="off" v-model="editData.assigned_user"/>
    </div>
    <div class="flex justify-end gap-2">
      <Button type="button" label="Cancel" severity="secondary" @click="showEdit = false"></Button>
      <Button type="button" label="Save" @click="editEntry()"></Button>
    </div>
  </Dialog>
</template>

<style scoped>

</style>