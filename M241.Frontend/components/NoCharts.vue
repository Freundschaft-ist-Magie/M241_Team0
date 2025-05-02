<script setup lang="ts">
defineProps<{
  selectedRoom: {
    roomId: string;
  };
}>();
</script>

<template>
  <div
    class="p-4 border border-dashed border-yellow-400 rounded-lg bg-yellow-50 dark:bg-yellow-900/50 text-center"
  >
    <p class="text-yellow-700 dark:text-yellow-300">
      Für den Raum "{{ selectedRoom.roomId }}" liegen derzeit keine Verlaufsdaten vor.
    </p>
    <p class="text-yellow-600 dark:text-yellow-400 text-sm mt-1">
      Diagramme und Tabelle werden angezeigt, sobald Daten verfügbar sind.
    </p>
  </div>
</template>
