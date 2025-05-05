<script lang="ts" setup>
import { getConfig } from "~/utils/helper/ConfigLoader";

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

const isLow = computed(() => props.value != null && props.value < props.normalRange.low);
const isHigh = computed(
  () => props.value != null && props.value > props.normalRange.high
);
const isCritical = computed(() => isLow.value || isHigh.value);

const showCriticalMessage = ref(false);
let criticalTimerId: ReturnType<typeof setTimeout> | null = null;
const CRITICAL_DELAY = ref(0);

watch(isCritical, (newValue, oldValue) => {
  if (criticalTimerId) {
    clearTimeout(criticalTimerId);
    criticalTimerId = null;
  }

  if (newValue === true) {
    criticalTimerId = setTimeout(() => {
      if (isCritical.value) {
        showCriticalMessage.value = true;
      }
      criticalTimerId = null;
    }, CRITICAL_DELAY.value);
  } else {
    showCriticalMessage.value = false;
  }
});

onMounted(async () => {
  const config = await getConfig();
  if (config) {
    CRITICAL_DELAY.value = config.countdown?.criticalDelaylMs;
  } else {
    console.error("Failed to load configuration");
  }
});

onUnmounted(() => {
  if (criticalTimerId) {
    clearTimeout(criticalTimerId);
  }
});
</script>

<template>
  <div
    class="p-4 shadow-md rounded-md text-black dark:text-darkNeutral2 h-full dark:bg-darkNeutral1 transition-colors duration-300 ease-in-out"
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
      class="bg-white dark:bg-darkNeutral1 rounded-md p-4 h-4/5 flex flex-col justify-center items-center text-center shadow shadow-black/60 transition-colors duration-300 ease-in-out relative overflow-hidden"
      :class="[
        isCritical
          ? 'bg-red-100 dark:bg-red-900/60'
          : 'bg-green-100 dark:bg-green-800/60',
      ]"
    >
      <!-- necessary for animation -->
      <div>
        <p class="text-3xl font-bold text-black dark:text-darkNeutral2">
          {{ props.value != null ? parseFloat(props.value.toFixed(2)) : "N/A" }}
          {{ unit }}
        </p>

        <p class="text-base text-gray-600 dark:text-darkSecondary2 mt-2">
          Normaler Bereich: {{ normalRange.low }} {{ unit }} – {{ normalRange.high }}
       {{ unit }}
        </p>

        <div class="w-full mt-4 min-h-[90px]">
          <div
            v-if="showCriticalMessage && props.value != null"
            class="text-amber-700 dark:text-amber-300"
          >
            <Message severity="error" class="dark:bg-darkNeutral1!">
              <i class="pi pi-exclamation-triangle mr-2"></i>
              <span v-if="isLow">{{ criticalText.low }}</span>
              <span v-else-if="isHigh">{{ criticalText.high }}</span>
            </Message>
          </div>
        </div>
      </div>

      <div
        v-if="isCritical && !showCriticalMessage"
        class="absolute bottom-0 left-0 w-full h-1 bg-gray-200 dark:bg-gray-700 overflow-hidden"
      >
        <div
          class="h-full bg-red-500 dark:bg-red-400 w-0"
          :style="{
            animation: `progressBarAnimation ${CRITICAL_DELAY}ms linear forwards`,
          }"
        />
      </div>
    </div>
  </div>
</template>

<style>
@keyframes progressBarAnimation {
  from {
    width: 0%;
  }
  to {
    width: 100%;
  }
}
</style>
