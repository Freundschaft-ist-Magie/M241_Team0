<script lang="ts" setup>
import { computed } from "vue";
import { FilterMatchMode } from '@primevue/core/api';

const props = defineProps({
  roomData: Array, // Das komplette Array (mit den Subarrays pro Raum)
});

const emit = defineEmits<{
  (e: "dataSelected", payload: any): void;
}>();

const selectedData = ref(null)
const filters = ref({
  global: { value: null, matchMode: FilterMatchMode.CONTAINS },
  id: { value: null, matchMode: FilterMatchMode.STARTS_WITH },
  humidity: { value: null, matchMode: FilterMatchMode.EQUALS },
  roomId: { value: null, matchMode: FilterMatchMode.EQUALS },
  temperature: { value: null, matchMode: FilterMatchMode.EQUALS },
  pressure: { value: null, matchMode: FilterMatchMode.EQUALS },
  airQuality: { value: null, matchMode: FilterMatchMode.EQUALS },
  timeStamp: { value: null, matchMode: FilterMatchMode.CONTAINS }
})


// Die Daten für das gewählte Room-Id herausfiltern:
const filteredRoomData = computed(() => {
  if (!props.roomData || Object.keys(props.roomData).length === 0) {
    return [];
  }

  // Alle Daten aus dem Objekt zusammenführen
  const allRoomsData = Object.entries(props.roomData).flatMap(([roomId, roomEntries]) =>
      roomEntries.map((entry, index) => ({
        id: `${roomId}-${index + 1}`, // Eine Kombination aus Raum-ID und Index als ID
        roomId: Number(roomId),      // Raum-ID hinzufügen (als Zahl)
        ...entry,
      }))
  );

  return allRoomsData.reverse();
});

function onRowSelect() {
  emit("dataSelected", selectedData.value)
}
</script>

<template>
  <Accordion value="0">
    <AccordionPanel value="0">
      <AccordionHeader>Archiv aller Räume</AccordionHeader>
      <AccordionContent>
        <DataTable
            :value="filteredRoomData"
            tableStyle="min-width: 50rem"
            paginator
            :rows="5"
            :rowsPerPageOptions="[5, 10, 20, 50]"
            @rowSelect="onRowSelect"
            selectionMode="single"
            dataKey="id"
            metaKeySelection=false
            v-model:selection="selectedData"
            v-model:filters="filters"
            :globalFilterFields="['id', 'roomId', 'humidity', 'temperature', 'pressure', 'airQuality', 'timeStamp']"
            filterDisplay="row"
        >
          <template #header>
            <div class="flex justify-end">
              <IconField>
                <InputIcon>
                  <i class="pi pi-search" />
                </InputIcon>
                <InputText v-model="filters['global'].value" placeholder="Keyword Search" />
              </IconField>
            </div>
          </template>
          <Column field="id" header="#" filterField="id" :showFilterMenu="false" >
            <template #filter="{ filterModel, filterCallback }">
              <InputText v-model="filterModel.value" type="text" @input="filterCallback()" placeholder="Search by ID" />
            </template>
          </Column>
          <Column field="roomId" header="Raum" filterField="roomId" :showFilterMenu="false" >
            <template #filter="{ filterModel, filterCallback }">
              <InputText v-model="filterModel.value" type="number" @input="filterCallback()" placeholder="Search by Room ID" />
            </template>
          </Column>
          <Column field="humidity" header="Feuchtigkeit"></Column>
          <Column field="temperature" header="Temperatur">
          </Column>
          <Column field="pressure" header="Druck"></Column>
          <Column field="airQuality" header="Luftqualität"></Column>
          <Column field="timeStamp" header="Zeitstempel" filterField="timeStamp" :showFilterMenu="true" >
            <template #filter="{ filterModel, filterCallback }">
              <InputText v-model="filterModel.value" type="datetime-local" @input="filterCallback()" placeholder="Search by Time" />
            </template>
          </Column>
        </DataTable>
      </AccordionContent>
    </AccordionPanel>
  </Accordion>
</template>
