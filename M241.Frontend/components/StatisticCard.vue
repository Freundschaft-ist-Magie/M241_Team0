<script lang="ts" setup>
const props = defineProps({
  title: String,
  value: Number,
  icon: String,
  unit: String,
  normalRange: {
    low: Number,
    high: Number,
  },
  criticalText: {
    low: String,
    high: String,
  },
});

const isLow = computed(() => props.value < props.normalRange.low);
const isHigh = computed(() => props.value > props.normalRange.high);
const isCritical = computed(() => isLow.value || isHigh.value);
</script>

<template>
  <div
    class="bg-white p-4 shadow-md shadow-black/40 rounded-md"
    :class="{ 'border border-red-600': isCritical }"
  >
    <div class="flex justify-between items-center mb-2">
      <h3 class="font-bold text-primary2" :class="{ 'text-red-600': isCritical }">
        {{ title }}
      </h3>
      <Icon :name="icon" class="text-3xl text-black" />
    </div>
    <p class="text-2xl font-bold">{{ parseFloat(value.toFixed(2)) }} {{ unit }}</p>
    <p class="text-gray-500">
      Normaler Bereich:
      <span> {{ normalRange.low }} {{ unit }} - {{ normalRange.high }} {{ unit }} </span>
    </p>

    <div v-if="isCritical" class="text-amber-700">
      <Message severity="error" variant="solid" class="my-4">
        <i class="pi pi-exclamation-triangle mr-2"></i>
        <span v-if="isLow">{{ criticalText.low }}</span>
        <span v-else-if="isHigh">{{ criticalText.high }}</span>
      </Message>
    </div>
  </div>
</template>
