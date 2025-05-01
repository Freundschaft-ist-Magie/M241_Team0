<script lang="ts" setup>
import RoomsService from "~/utils/services/RoomsService";
import GlobalHelper from "~/utils/helper/GlobalHelper";
import type ChartData from "~/models/ChartData";
import ChartOptions from "~/models/ChartOptions";
import Room from "~/models/Room";
import { useToast } from "primevue/usetoast";

const toast = useToast();
const loading = ref(true);
const latestFetch = ref(new Date());
const selectedRoom = ref<Room | null>(null);
const rooms = ref<Room[]>([]);
const selectedMetric = ref("Temperatur");
const charts = ref<{ data: ChartData; options: ChartOptions }[]>([]);
const metrics = [
  { label: "Temperatur", value: "Temperatur" },
  { label: "Luftfeuchtigkeit", value: "Luftfeuchtigkeit" },
  { label: "CO₂ Level", value: "CO₂ Level" },
];

// Add computed property to get the current chart based on selected metric
const currentChart = computed(() => {
  if (!selectedMetric.value || charts.value.length === 0) return null;

  if (selectedMetric.value === "Temperatur") {
    return charts.value[0];
  } else if (selectedMetric.value === "Luftfeuchtigkeit") {
    return charts.value[1];
  } else if (selectedMetric.value === "CO₂ Level") {
    return charts.value[2];
  }

  return null;
});

const changeMetric = (metric: string) => {
  selectedMetric.value = metric;
};

const showErrorToast = (summary: string, detail: string, life: number = 5000) => {
  toast.add({
    severity: "error",
    summary: summary,
    detail: detail,
    life: life,
  });
};

const showWarningToast = (summary: string, detail: string, life: number = 5000) => {
  toast.add({
    severity: "warn",
    summary: summary,
    detail: detail,
    life: life,
  });
};

onMounted(async () => {
  loading.value = true;

  try {
    const roomsService = new RoomsService();
    const fetchedRooms = await roomsService.Get();
    const forecasts = await roomsService.GetForecasts();

    if (!fetchedRooms.length) {
      showErrorToast("Fehler", "Keine Räume gefunden.");
      loading.value = false;
      return;
    }

    // Update rooms with fetched data
    rooms.value = fetchedRooms.map((room: any) => {
      return new Room(room.roomId, room.name, room.description, {
        temperature: room.environmentData.temperature,
        humidity: room.environmentData.humidity,
        airQuality: room.environmentData.airQuality,
      });
    });

    selectedRoom.value = rooms.value[0];

    // Find the forecast for the selected room
    const roomForecast = forecasts.find((f) => f.roomId === selectedRoom.value?.id);

    if (!roomForecast) {
      showErrorToast(
        "Fehler",
        `Keine Prognosedaten für Raum "${selectedRoom.value.name}" gefunden.`
      );
      console.error(`No forecast found for room with ID ${selectedRoom.value.roomId}`);
      loading.value = false;
      return;
    }

    // Check if forecast data exists for each metric
    const hasTempForecast =
      Array.isArray(roomForecast.environmentData?.temperature) &&
      roomForecast.environmentData.temperature.length > 0;
    const hasHumidityForecast =
      Array.isArray(roomForecast.environmentData?.humidity) &&
      roomForecast.environmentData.humidity.length > 0;
    const hasAirQualityForecast =
      Array.isArray(roomForecast.environmentData?.airQuality) &&
      roomForecast.environmentData.airQuality.length > 0;

    if (!hasTempForecast && !hasHumidityForecast && !hasAirQualityForecast) {
      showErrorToast(
        "Fehler",
        `Keine Prognosedaten für Raum "${selectedRoom.value.name}" verfügbar.`
      );
      loading.value = false;
      return;
    }

    // Create combined charts for temperature, humidity, and air quality
    const temperatureCombinedData = GlobalHelper.MapCombinedChartData(
      selectedRoom.value.environmentData.temperature,
      roomForecast.environmentData.temperature || [],
      "temperature"
    );

    const humidityCombinedData = GlobalHelper.MapCombinedChartData(
      selectedRoom.value.environmentData.humidity,
      roomForecast.environmentData.humidity || [],
      "humidity"
    );

    const airQualityCombinedData = GlobalHelper.MapCombinedChartData(
      selectedRoom.value.environmentData.airQuality,
      roomForecast.environmentData.airQuality || [],
      "airQuality"
    );

    const chartOptions = new ChartOptions();

    charts.value = [
      { data: temperatureCombinedData, options: chartOptions },
      { data: humidityCombinedData, options: chartOptions },
      { data: airQualityCombinedData, options: chartOptions },
    ];

    // Warning for specific metrics without forecast data
    if (!hasTempForecast || !hasHumidityForecast || !hasAirQualityForecast) {
      const missingForecasts = [];
      if (!hasTempForecast) missingForecasts.push("Temperatur");
      if (!hasHumidityForecast) missingForecasts.push("Luftfeuchtigkeit");
      if (!hasAirQualityForecast) missingForecasts.push("CO₂ Level");

      showWarningToast(
        "Hinweis",
        `Keine Prognosedaten verfügbar für: ${missingForecasts.join(", ")}`
      );
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    showErrorToast(
      "Fehler",
      "Fehler beim Laden der Daten. Bitte versuchen Sie es später erneut."
    );
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <div class="flex flex-col gap-8">
    <Toast position="top-right" />

    <RoomSelectorCard
      :latestFetch="latestFetch"
      :selectedRoom="selectedRoom"
      :rooms="rooms"
    />

    <div class="flex justify-center mb-4">
      <ButtonGroup>
        <Button
          v-for="metric in metrics"
          :key="metric.value"
          :label="metric.label"
          :severity="selectedMetric === metric.value ? 'primary' : 'info'"
          class="px-4 py-2 text-gray-700"
          @click="changeMetric(metric.value)"
        />
      </ButtonGroup>
    </div>

    <div class="flex justify-center">
      <div class="w-3/4 bg-gray-300 p-6 rounded-lg shadow-md">
        <div v-if="loading" class="flex items-center justify-center h-40">
          <span class="text-gray-700">Daten werden geladen...</span>
        </div>
        <div v-else-if="currentChart" class="relative">
          <StatisticDiagram
            :title="selectedMetric"
            :chartData="currentChart.data"
            :chartOptions="currentChart.options"
            chartType="line"
          />
        </div>
        <div v-else class="flex items-center justify-center h-40">
          <span class="text-gray-700">Keine Daten verfügbar</span>
        </div>
      </div>
    </div>
  </div>
</template>
