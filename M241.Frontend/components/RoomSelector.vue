<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';

const props = defineProps<{
  options: Array<any>;
  placeholder: string;
  filterField: string;
  selectedRoom?: any;
}>();

const emit = defineEmits<{
  (e: 'roomSelected', payload: any): void;
}>();

// 1. Model initialisieren mit props.selectedRoom oder fallback auf options[0]
const model = ref<any>(
    props.selectedRoom ?? (props.options.length > 0 ? props.options[0] : null)
);

// 2. Sobald das Model sich ändert, Event nach oben emitten
watch(model, (newVal) => {
  if (newVal) emit('roomSelected', newVal);
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
      class="w-full md:w-sm bg-white! border-0! shadow-none!"
  >
    <template #value="slotProps">
      <div v-if="slotProps.value" class="flex items-center">
        <div class="text-3xl font-bold text-black">
          Room {{ slotProps.value.roomId }}
        </div>
      </div>
      <span v-else class="text-3xl font-bold text-black">
        {{ options && options.length > 0 ? "Room " + options[0].roomId : "No data" }}
      </span>
    </template>
    <template #option="slotProps">
      <div class="flex items-center">
        <div>Room {{ slotProps.option.roomId }}</div>
      </div>
    </template>
  </Select>
</template>
