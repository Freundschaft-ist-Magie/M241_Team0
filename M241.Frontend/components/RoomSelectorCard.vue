<script lang="ts" setup>
import Room from "~/models/Room";
import RoomSelector from "~/components/RoomSelector.vue";
import { defineEmits } from "vue";

defineProps<{
  latestFetch: Date;
  rooms: Room[];
  selectedRoom: Room;
  countdown: number;
  isPingPending: boolean;
}>();

const emit = defineEmits(["roomSelected", "pingRoom"]);

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
    class="mt-4 p-4 bg-white shadow-md shadow-black/40 rounded-md flex justify-between items-center dark:bg-darkNeutral1 dark:text-darkNeutral2"
  >
    <div class="flex-grow">
      <RoomSelector
        :options="rooms"
        :placeholder="'Raum auswählen'"
        :filter-field="'roomId'"
        :selectedRoom="selectedRoom"
        @roomSelected="handleRoomSelected"
      />
      <p class="text-gray-500 dark:text-darkSecondary2">
        Zuletzt aktualisiert: {{ formatDate(latestFetch) }}
      </p>
      <p v-if="countdown >= 0" class="text-gray-400 text-sm dark:text-darkSecondary2">
        Charts werden aktualisiert in {{ countdown }} Sekunden.
      </p>
    </div>

    <Button
      v-if="selectedRoom"
      icon="pi pi-wifi"
      @click="$emit('pingRoom', selectedRoom)"
      :loading="isPingPending"
      severity="secondary"
      tooltip="Ping Room"
      rounded
      raised
    />
  </div>
</template>
