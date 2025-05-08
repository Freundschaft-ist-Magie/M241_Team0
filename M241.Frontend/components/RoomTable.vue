<script lang="ts" setup>
import { computed } from "vue";
import GlobalHelper from "../utils/helper/GlobalHelper";

const props = defineProps({
  roomData: Array, // Das komplette Array (mit den Subarrays pro Raum)
  selectedRoom: Object,
});

// Die Daten für das gewählte Room-Id herausfiltern:
const filteredRoomData = computed(() => {
  if (props.selectedRoom == undefined) {
    return [];
  }

  console.warn(props.selectedRoom);

  const data = props.roomData[props.selectedRoom.roomId];
  if (!data) return [];
  // Optional: Id hinzufügen, wenn gewünscht
  return data.map((entry, index) => ({
    id: index + 1,
    ...entry
  })).reverse();

});
</script>

<template>
  <Accordion v-if="props.selectedRoom" value="0">
    <AccordionPanel value="0">
      <AccordionHeader>Raumtabelle für Raum {{ props.selectedRoom.room.macAddress }}</AccordionHeader>
      <AccordionContent>
        <DataTable
            :value="filteredRoomData"
            tableStyle="min-width: 50rem"
            paginator
            :rows="5"
            :rowsPerPageOptions="[5, 10, 20, 50]"
        >
          <Column field="id" header="#"></Column>
          <Column field="humidity" header="Feuchtigkeit"></Column>
          <Column field="temperature" header="Temperatur"></Column>
          <Column field="pressure" header="Druck"></Column>
          <Column field="airQuality" header="Luftqualität"></Column>
          <Column field="timeStamp" header="Zeitstempel">
            <template #body="slotProps">
              {{ GlobalHelper.beautifyDate(slotProps.data.timeStamp) }}
              </template>
          </Column>
        </DataTable>
      </AccordionContent>
    </AccordionPanel>
  </Accordion>
</template>
