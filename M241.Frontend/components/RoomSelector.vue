<script setup lang="ts">
import { ref, watch, onMounted } from "vue";

const props = defineProps<{
  options: Array<any>;
  placeholder: string;
  filterField: string;
  selectedRoom?: any;
}>();

const emit = defineEmits<{
  (e: "roomSelected", payload: any): void;
}>();

// 1. Model initialisieren mit props.selectedRoom oder fallback auf options[0]
const model = ref<any>(
  props.selectedRoom ?? (props.options.length > 0 ? props.options[0] : null)
);

// 2. Sobald das Model sich ändert, Event nach oben emitten
watch(model, (newVal) => {
  if (newVal) emit("roomSelected", newVal);
});

// 3. Wenn props.options später neu kommen und model noch leer ist, default setzen
watch(
  () => props.options,
  (newOptions) => {
    if ((!model.value || !newOptions.includes(model.value)) && newOptions.length > 0) {
      model.value = newOptions[0];
    }
  },
  { immediate: true }
);

onMounted(() => {
  // Falls du props.selectedRoom nicht nutzt, kannst du hier auch nochmal ein Fallback setzen
  if (!model.value && props.options.length > 0) {
    model.value = props.options[0];
  }
});
</script>

<template>
  <Select
    v-model="model"
    :options="options"
    filter
    :optionLabel="filterField"
    :placeholder="placeholder"
    size="large"
    class="w-full md:w-sm bg-white! dark:bg-darkNeutral1! border-0! shadow-none! text-black dark:text-darkNeutral2"
  >
    <template #value="slotProps">
      <div v-if="slotProps.value" class="flex items-center">
        <div class="text-3xl font-bold text-black dark:text-darkNeutral2">
          Raum
          {{ slotProps.value.room.name || slotProps.value.room.macAddress }}
        </div>
      </div>
      <span v-else class="text-3xl font-bold text-black dark:text-darkNeutral2">
        {{
          options && options.length > 0
            ? "Room " + (options[0].room.name || options[0].room.macAddress)
            : "Keine Räume"
        }}
      </span>
    </template>
    <template #option="slotProps">
      <div class="flex items-center text-black dark:text-darkNeutral2">
        Raum
        <template v-if="slotProps.option.room.name">
          {{ " " + slotProps.option.room.name }}
        </template>
        <template v-if="slotProps.option.room.name && slotProps.option.room.macAddress">
          ,
        </template>
        <template v-if="slotProps.option.room.macAddress">
          {{ " " + slotProps.option.room.macAddress }}
        </template>
      </div>
    </template>
  </Select>
</template>
