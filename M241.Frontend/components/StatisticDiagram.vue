<script setup lang="ts">
import type ChartData from "~/models/ChartData";
import type ChartOptions from "~/models/ChartOptions";

defineProps<{
  title: string;
  chartData: ChartData;
  chartOptions: ChartOptions;
  chartType: string;
}>();
</script>

<template>
  <div
    class="bg-white dark:bg-darkNeutral1 p-4 shadow-md shadow-black/40 rounded-md text-black dark:text-darkNeutral2"
  >
    <h3 class="font-bold text-primary2 dark:text-darkPrimary1 text-center">
      {{ title }}
    </h3>
    <Chart :type="chartType" :data="chartData" :options="chartOptions" />
  </div>
</template>
