<script lang="ts" setup>
import { useLoadingStore } from "@/utils/stores/base/LoadingStore";
import GlobalHelper from "~/utils/helper/GlobalHelper";
import StatisticCardObj from "~/models/StatisticCardObj";
import ChartData from "~/models/ChartData";
import ChartOptions from "~/models/ChartOptions";
import { useRoomDataStore } from "~/utils/stores/RoomDataStore";
import { useRoomStore } from "~/utils/stores/RoomStore";
import RoomData from "~/models/RoomData";

// ----- STORE INTEGRATION -----
const loadingStore = useLoadingStore();
const roomStore = useRoomStore();
const roomDataStore = useRoomDataStore();

// ----- Interfaces -----

interface DisplayRoom {
  roomId: string;
  temperature: number;
  humidity: number;
  pressure: number;
  gas: number;
  timeStamp: string;
}

interface ProcessedHistoryEntry {
  timeStamp: string;
  temperature: number;
  humidity: number;
  pressure: number;
  airQuality: number;
}

// ----- COMPONENT LOGIC -----

type MessageCallback = (data: any) => void;

const latestFetch = ref<Date>(new Date(1, 1, 1970));
const selectedRoom = ref<DisplayRoom | null>(null);
const rooms = ref<DisplayRoom[]>([]);
const roomsHistory = ref<Record<string, ProcessedHistoryEntry[]>>({});
const cards = ref<StatisticCardObj[]>([]);
const charts = ref<{ data: ChartData; options: ChartOptions }[]>([]);
const chartTitles = ref([
  "Temperatur (letzte Daten)",
  "Luftfeuchtigkeit (letzte Daten)",
  "Luftqualität (letzte Daten)",
  "Luftdruck (letzte Daten)",
]);
const tabs = ref([
  { title: "Temperatur", value: "0", dayChart: 0 },
  { title: "Luftfeuchtigkeit", value: "1", dayChart: 1 },
  { title: "CO₂ Level", value: "2", dayChart: 2 },
  { title: "Luftdruck", value: "3", dayChart: 3 },
]);

loadingStore.setLoading(true);

const hasRooms = computed(() => rooms.value.length > 0);

const hasHistoryDataForSelectedRoom = computed(() => {
  if (!selectedRoom.value) return false;
  const history = roomsHistory.value[selectedRoom.value.roomId];
  return history && history.length > 0;
});

// ----- DATA PROCESSING FUNCTIONS -----

/**
 * Processes raw RoomData[] fetched from the store into structured
 * history and a list of rooms with their latest data point.
 */
function processFetchedData(allRoomData: RoomData[]) {
  const historyMap: Record<string, ProcessedHistoryEntry[]> = {};
  const latestDataMap: Record<string, RoomData> = {};

  // Ensure data is sorted by timestamp if not guaranteed by API
  // Convert string timestamps to Date objects for reliable comparison
  allRoomData.sort(
      (a, b) => new Date(a.timeStamp).getTime() - new Date(b.timeStamp).getTime()
  );

  for (const dataPoint of allRoomData) {
    const roomIdStr = String(dataPoint.roomId);

    // 1. Build History Map
    if (!historyMap[roomIdStr]) {
      historyMap[roomIdStr] = [];
    }
    historyMap[roomIdStr].push({
      timeStamp: dataPoint.timeStamp,
      temperature: dataPoint.temperature,
      humidity: dataPoint.humidity,
      pressure: dataPoint.pressure,
      airQuality: dataPoint.gas,
    });

    // 2. Track Latest Data Point (since data is sorted, last one wins)
    latestDataMap[roomIdStr] = dataPoint;
  }

  // 3. Create DisplayRoom list from latest data
  const displayRooms: DisplayRoom[] = Object.values(latestDataMap).map((latest) => ({
    roomId: String(latest.roomId),
    temperature: latest.temperature,
    humidity: latest.humidity,
    pressure: latest.pressure,
    gas: latest.gas,
    timeStamp: latest.timeStamp,
    room: latest.room
  }));

  displayRooms.sort((a, b) => a.roomId.localeCompare(b.roomId));

  // Update component state
  rooms.value = displayRooms;
  roomsHistory.value = historyMap;
  latestFetch.value = new Date();
}

// ----- UI UPDATE FUNCTIONS -----

function setCards() {
  cards.value = [];
  if (!selectedRoom.value) return;

  const room = selectedRoom.value;
  console.log("Setting cards for room:", room.roomId);

  const cardTemperature = GlobalHelper.MapTemperature(room.temperature);
  const cardHumidity = GlobalHelper.MapHumidity(room.humidity);
  const cardAirQuality = GlobalHelper.MapAirQuality(room.airQuality);
  const cardPressure = GlobalHelper.MapPressure(room.pressure);
  const cardCompGas = GlobalHelper.MapCompGas(room.airQuality, room.humidity);

  if (cardTemperature.value !== undefined) {
    cards.value = [
      cardTemperature,
      cardHumidity,
      cardAirQuality,
      cardPressure,
      cardCompGas,
    ];
  }
}

function initializeCharts() {
  charts.value = [];
  if (!selectedRoom.value?.roomId) return;

  const roomId = selectedRoom.value.roomId;
  const historyForSelected = roomsHistory.value[roomId] ?? [];

  if (historyForSelected.length > 0) {
    console.log(
        "Initializing charts for room:",
        roomId,
        "with",
        historyForSelected.length,
        "history points"
    );

    const temperatureData = GlobalHelper.MapChartDataTemperature(historyForSelected);
    const humidityData = GlobalHelper.MapChartDataHumidity(historyForSelected);
    const airQualityData = GlobalHelper.MapChartDataAirQuality(historyForSelected);
    const pressureData = GlobalHelper.MapChartDataPressure(historyForSelected);

    const chartOptions = new ChartOptions();

    charts.value.push(
        { data: temperatureData, options: chartOptions },
        { data: humidityData, options: chartOptions },
        { data: airQualityData, options: chartOptions },
        { data: pressureData, options: chartOptions }
    );
  } else {
    console.log("No history data to initialize charts for room:", roomId);

    const emptyChartData = (): ChartData => ({
      labels: [],
      datasets: [{ label: "", data: [] as Array }],
    });
    const chartOptions = new ChartOptions();
    charts.value.push(
        { data: emptyChartData(), options: chartOptions },
        { data: emptyChartData(), options: chartOptions },
        { data: emptyChartData(), options: chartOptions },
        { data: emptyChartData(), options: chartOptions }
    );
    console.log("Initialized empty chart structures for room:", roomId);
  }
}

// ----- LIFECYCLE HOOK -----

onMounted(async () => {
  console.log("Archive Mounted - Fetching initial data from stores...");
  loadingStore.setLoading(true);
  selectedRoom.value = null; // Reset selection
  rooms.value = [];
  roomsHistory.value = {};

  try {
    const rawRoomData = await roomDataStore.GetAll();

    if (!rawRoomData || rawRoomData.length === 0) {
      console.warn("No room data received from the store.");
    } else {
      console.log(`Fetched ${rawRoomData.length} data points.`);
      processFetchedData(rawRoomData);

      if (!hasRooms.value) {
        console.log("No displayable rooms after processing data.");
      }
    }
  } catch (error) {
    console.error("Failed to fetch or process initial room data:", error);
    rooms.value = [];
    roomsHistory.value = {};
  } finally {
    loadingStore.setLoading(false);
    console.log("Initial data fetch attempt complete.");
  }
});

// Watch for changes in selectedRoom to update UI
watch(
    selectedRoom,
    (newRoom, oldRoom) => {
      if (newRoom?.roomId !== oldRoom?.roomId) {
        console.log(`Selected room changed from ${oldRoom?.roomId} to ${newRoom?.roomId}`);

        // Unsubscribe happens inside subscribeToRoom, but calling it here for clarity

        setCards();
        initializeCharts();
      }
    },
    { immediate: false }
);

function onDataSelected(data) {
  selectedRoom.value = data
  setCards()
}
</script>

<template>
  <div v-if="loadingStore.isLoading">
    <Loading class="mt-12" />
  </div>

  <!-- Fallback 1: No Rooms -->
  <NoRooms v-else-if="!hasRooms" />

  <!-- Main content -->
  <div v-else id="main-content">

    <ArchiveRoomTable :room-data="roomsHistory" :selected-room="selectedRoom" class="mt-4" @dataSelected="onDataSelected"  />

    <!-- Statistic Cards -->
    <div
        v-if="selectedRoom"
        class="mt-4 grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 items-center"
    >
      <StatisticCard
          v-for="card in cards"
          :key="card.title"
          class="stat-card"
          :title="card.title"
          :value="card.value"
          :icon="card.icon"
          :unit="card.unit"
          :normalRange="card.normalRange"
          :criticalText="card.criticalText"
      />
    </div>

    <!-- Placeholder if no room is selected -->
    <div
        v-else
        class="mt-4 p-4 bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-300 rounded-md text-center"
    >
      Bitte wählen Sie oben einen Raum aus, um Details anzuzeigen.
    </div>

    <!-- Data Display Area -->
    <div v-if="selectedRoom" class="mt-4">
      <!-- Fallback 2: No History Data for the selected room -->
      <NoCharts v-if="!hasHistoryDataForSelectedRoom" :selectedRoom="selectedRoom" />

      <!-- Display Table and Charts -->
      <div v-else>
        <!-- Room Table -->
        <RoomTable :room-data="roomsHistory" :selected-room="selectedRoom" class="mt-4" />

        <!-- Mobile Tabs for Charts -->
        <div class="mt-4 block sm:hidden">
          <Tabs value="0">
            <TabList
                class="flex border-b border-gray-200 dark:border-gray-700 overflow-x-auto"
            >
              <Tab
                  v-for="tab in tabs"
                  :key="tab.title"
                  :value="tab.value"
                  v-slot="{ selected }"
                  as="template"
              >
                <button
                    :class="[
                    'px-4 py-2 text-sm font-medium leading-5 whitespace-nowrap',
                    'focus:outline-none focus:ring-2 ring-offset-1 ring-offset-blue-400 ring-white ring-opacity-60',
                    selected
                      ? 'text-blue-700 bg-blue-100 dark:text-blue-300 dark:bg-blue-900 border-b-2 border-blue-500'
                      : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200',
                  ]"
                >
                  {{ tab.title }}
                </button>
              </Tab>
            </TabList>
            <TabPanels class="mt-2">
              <TabPanel
                  v-for="tab in tabs"
                  :key="tab.value + '-panel'"
                  :value="tab.value"
                  class="p-0 focus:outline-none"
              >
                <div class="mt-4 flex flex-col gap-4">
                  <!-- Display Day Chart for the selected tab -->
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
              :key="'day-' + index"
              :title="chartTitles[index]"
              :chartData="chart.data"
              :chartOptions="chart.options"
              chartType="line"
              class="flex-grow w-full md:w-[calc(50%-0.5rem)] min-w-[250px]"
          />
          <!-- Placeholder if charts array is empty but history exists (edge case) -->
          <p v-if="charts.length === 0" class="w-full text-center text-gray-500 py-4">
            Keine Diagramme zum Anzeigen.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.stat-card {
  transition: box-shadow 0.3s ease;
}
.stat-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

button:focus {
  outline: none;
}
</style>
