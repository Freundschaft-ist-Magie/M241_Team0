<script lang="ts" setup>
import Room from "~/models/Room";
import RoomSelector from "~/components/RoomSelector.vue"; // falls nötig importieren
import { defineEmits } from "vue";

defineProps<{
  latestFetch: Date;
  rooms: Room[];
  selectedRoom: Room;
  countdown: number;
}>();

const emit = defineEmits(["roomSelected"]); // Das Event definieren

function formatDate(date: Date) {
  return date.toLocaleString("de-CH", {
    month: "short",
    day: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

// Das Event einfach weiterleiten:
function handleRoomSelected(room: Room) {
  emit("roomSelected", room);
}
</script>

<template>
  <div
    class="mt-4 p-4 bg-white shadow-md shadow-black/40 rounded-md flex justify-between items-center"
  >
    <div>
      <RoomSelector
        :options="rooms"
        :placeholder="'Raum auswählen'"
        :filter-field="'roomId'"
        :selectedRoom="selectedRoom"
        @roomSelected="handleRoomSelected"
      />
      <p class="text-gray-500">Zuletzt aktualisiert: {{ formatDate(latestFetch) }}</p>
      <p v-if="countdown >= 0" class="text-gray-400 text-sm">
        Charts werden aktualisiert in {{ countdown }} Sekunden.
      </p>
    </div>
  </div>
</template>
