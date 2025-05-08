<script setup lang="ts">
defineProps<{
  tabs: Array<{
    title: string;
    value: string;
    dayChart: number;
  }>;
  charts: Array<{
    data: any;
    options: any;
  }>;
  chartTitles: Array<string>;
}>();
</script>

<template>
  <!-- Mobile Tabs for Charts -->
  <div class="mt-4 block sm:hidden">
    <Tabs value="0">
      <TabList class="flex border-b border-gray-200 dark:border-gray-700 overflow-x-auto">
        <Tab v-for="tab in tabs" :key="tab.title" :value="tab.value">
          {{ tab.title }}
        </Tab>
      </TabList>
      <TabPanels class="p-0!">
        <TabPanel v-for="tab in tabs" :key="tab.value" :value="tab.value">
          <div class="mt-4 flex flex-col gap-4">
            <StatisticDiagram
              v-if="charts[tab.dayChart]"
              :title="chartTitles[tab.dayChart]"
              :chartData="charts[tab.dayChart].data"
              :chartOptions="charts[tab.dayChart].options"
              chartType="line"
            />
            <p v-else class="text-sm text-gray-500 text-center py-4">
              Diagramm nicht verfügbar.
            </p>
          </div>
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>

  <!-- Desktop Day Charts -->
  <div class="mt-4 hidden sm:flex flex-wrap gap-4">
    <StatisticDiagram
      v-for="(chart, index) in charts"
      :key="index"
      :title="chartTitles[index]"
      :chartData="chart.data"
      :chartOptions="chart.options"
      chartType="line"
      class="flex-grow w-full md:w-[calc(50%-0.5rem)] min-w-[250px]"
    />
    <p v-if="charts.length === 0" class="w-full text-center text-gray-500 py-4">
      Diagramme nicht verfügbar.
    </p>
  </div>
</template>
