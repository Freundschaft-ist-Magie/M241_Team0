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
    class="p-4 shadow-md rounded-md text-black dark:text-darkNeutral2 h-[300px] dark:bg-darkNeutral1 transition-colors duration-300 ease-in-out"
    :class="[isCritical ? 'bg-red-100' : 'bg-green-100']"
  >
    <div class="flex justify-between items-start mb-4">
      <div class="flex items-center gap-2">
        <Icon :name="icon" class="text-3xl text-black dark:text-darkNeutral2" />
        <h3
          class="font-bold text-lg text-primary2 dark:text-darkPrimary1 transition-colors duration-300 ease-in-out"
          :class="{ 'text-red-600': isCritical }"
        >
          {{ title }}
        </h3>
      </div>

      <Button
        icon="pi pi-ellipsis-v"
        severity="secondary"
        variant="text"
        rounded
        class="text-black! dark:text-white!"
      />
    </div>

    <div
      class="bg-white dark:bg-darkNeutral1 rounded-md p-4 h-4/5 flex flex-col justify-center items-center text-center shadow shadow-black/60 transition-colors duration-300 ease-in-out"
      :class="[
        isCritical ? 'bg-red-100 dark:bg-red-900/60' : 'bg-green-100 dark:bg-green-800/60',
      ]"
    >
      <p class="text-3xl font-bold text-black dark:text-darkNeutral2">
        {{ parseFloat(value.toFixed(2)) }} {{ unit }}
      </p>

      <p class="text-base text-gray-600 dark:text-darkSecondary2 mt-2">
        Normaler Bereich: {{ normalRange.low }} {{ unit }} – {{ normalRange.high }}
        {{ unit }}
      </p>

      <div v-if="isCritical" class="text-amber-700 dark:text-amber-300">
        <Message severity="error" variant="solid" class="my-4 dark:bg-darkNeutral1!">
          <i class="pi pi-exclamation-triangle mr-2"></i>
          <span v-if="isLow">{{ criticalText.low }}</span>
          <span v-else-if="isHigh">{{ criticalText.high }}</span>
        </Message>
      </div>
    </div>
  </div>
</template>
