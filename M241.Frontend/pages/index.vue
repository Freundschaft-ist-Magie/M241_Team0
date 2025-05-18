<script lang="ts" setup>
import { useLoadingStore } from "@/utils/stores/base/LoadingStore";
import GlobalHelper from "~/utils/helper/GlobalHelper";
import StatisticCardObj from "~/models/StatisticCardObj";
import ChartData from "~/models/ChartData";
import ChartOptions from "~/models/ChartOptions";
import { useRoomDataStore } from "~/utils/stores/RoomDataStore";
import { useRoomStore } from "~/utils/stores/RoomStore";
import { getConfig } from "~/utils/helper/ConfigLoader";
import SocketService from "~/utils/services/base/SocketService";
import RoomData from "~/models/RoomData";
import { useMinimizedStore } from "~/utils/stores/base/MinimizedStore";
import { useToastStore } from "~/utils/stores/base/ToastStore";

// ----- STORE INTEGRATION -----
const loadingStore = useLoadingStore();
const minimizedStore = useMinimizedStore();
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

const socketService = new SocketService();
let currentWsEndpoint: string | null = null;
type MessageCallback = (data: any) => void;
let currentWsCallback: MessageCallback | null = null;

const lastChartUpdate = ref<Date | null>(null);

const latestFetch = ref<Date>(new Date(1, 1, 1970));
const isBurning = ref(false);
const isPingPending = ref(false);
const selectedRoom = ref<DisplayRoom | null>(null);
const countdown = ref(0);
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

  console.log("allRoomData", allRoomData);

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
    room: latest.room,
  }));

  displayRooms.sort((a, b) => a.roomId.localeCompare(b.roomId));

  // Update component state
  rooms.value = displayRooms;
  roomsHistory.value = historyMap;
  latestFetch.value = new Date();
  selectedRoom.value = historyMap[selectedRoom.roomId];
}

// ----- WebSocket -----
/**
 * Processes a new RoomData object received via WebSocket.
 * Updates component state and triggers UI refreshes efficiently.
 */
function handleWebSocketMessage(newData: RoomData) {
  // DEBUG-Test
  console.log(`[WS] Received data for room ${newData.roomId}:`, newData);
  console.log("Selected room:", selectedRoom.value?.roomId);

  if (!selectedRoom.value || String(newData.roomId) !== selectedRoom.value.roomId) {
    console.warn(
      `[WS] Received data for room ${newData.roomId}, but room ${selectedRoom.value?.roomId} is selected. Ignoring.`
    );
    return;
  }

  console.log(`[WS] Received data for selected room ${newData.roomId}:`, newData);
  latestFetch.value = new Date();

  const roomIdStr = String(newData.roomId);

  // 1. Update the 'rooms' array (for RoomSelectorCard display)
  const roomIndex = rooms.value.findIndex((r) => r.roomId === roomIdStr);
  if (roomIndex !== -1) {
    const updatedDisplayRoom: DisplayRoom = {
      ...rooms.value[roomIndex],
      temperature: newData.temperature,
      humidity: newData.humidity,
      pressure: newData.pressure,
      gas: newData.gas,
      timeStamp: newData.timeStamp,
    };
    rooms.value.splice(roomIndex, 1, updatedDisplayRoom);

    console.error("Updated room:", newData);

    // Also update the properties of the selectedRoom ref directly.
    // This is what cards and potentially other parts of the UI bind to.
    selectedRoom.value.temperature = newData.temperature;
    selectedRoom.value.humidity = newData.humidity;
    selectedRoom.value.pressure = newData.pressure;
    selectedRoom.value.gas = newData.gas;
    selectedRoom.value.timeStamp = newData.timeStamp;
  } else {
    console.warn(
      `[WS] Received data for room ${roomIdStr}, but it's not in the 'rooms' list.`
    );
  }

  // 2. Add new data point to history
  const historyEntry: ProcessedHistoryEntry = {
    timeStamp: newData.timeStamp,
    temperature: newData.temperature,
    humidity: newData.humidity,
    pressure: newData.pressure,
    airQuality: newData.gas,
    isBurning: newData.isBurning,
  };
  if (!roomsHistory.value[roomIdStr]) {
    roomsHistory.value[roomIdStr] = [];
  }

  console.log("roomsHistory", historyEntry);
  isBurning.value = historyEntry.isBurning;
  console.log("isBurning", historyEntry.isBurning);

  roomsHistory.value[roomIdStr].push(historyEntry);
  // Optional: Limit history size in memory
  // const MAX_HISTORY_POINTS = 500;
  // if (roomsHistory.value[roomIdStr].length > MAX_HISTORY_POINTS) {
  //   roomsHistory.value[roomIdStr].shift(); // Remove the oldest point
  // }

  setCards();
  // let this code be in case we will have a chart in the future, that can be live updated
  // updateChartsWithNewData();
}

/**
 * Updates the existing chart data arrays instead of rebuilding them completely.
 */
function updateChartsWithNewData() {
  if (charts.value.length !== 4 || !selectedRoom.value) {
    console.warn(
      "[WS] Chart structure not ready or data for wrong room. Skipping direct chart update."
    );

    return;
  }

  charts.value = [];

  const roomId = selectedRoom.value?.roomId;
  const historyForSelected = roomId != null ? roomsHistory.value[roomId] ?? [] : [];

  if (historyForSelected.length > 0) {
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
  }
}

// ----- UI UPDATE FUNCTIONS -----

function setCards() {
  cards.value = [];
  if (!selectedRoom.value) return;

  const room = selectedRoom.value;
  console.error("Setting cards for room:", room.roomId, "with data:", room);

  const cardTemperature = GlobalHelper.MapTemperature(room.temperature);
  const cardHumidity = GlobalHelper.MapHumidity(room.humidity);
  const cardAirQuality = GlobalHelper.MapAirQuality(room.gas);
  const cardPressure = GlobalHelper.MapPressure(room.pressure);
  const cardCompGas = GlobalHelper.MapCompGas(room.gas, room.humidity);

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

function startCountdown(countdownSeconds: number) {
  countdown.value = countdownSeconds;
  setInterval(() => {
    countdown.value--;
    if (countdown.value <= 0) {
      countdown.value = countdownSeconds;
    }
  }, 1000);
}

// Add ping room handler
async function handlePingRoom(room: DisplayRoom) {
  try {
    isPingPending.value = true;
    const result = await roomStore.PingRoom(room.room.macAddress);

    // since result is empty, we can assume the ping was successful
    useToastStore().setToast(
      "success",
      "Ping erfolgreich",
      `Raum ${room.roomId} ist erreichbar`
    );
  } catch (error) {
    console.error("Failed to ping room:", error);
    useToastStore().setToast(
      "error",
      "Ping fehlgeschlagen",
      `Raum ${room.roomId} ist nicht erreichbar`
    );
  } finally {
    isPingPending.value = false;
  }
}

// ----- LIFECYCLE HOOK -----

onMounted(async () => {
  console.log("Index Page Mounted - Fetching initial data from stores...");
  loadingStore.setLoading(true);
  selectedRoom.value = null; // Reset selection
  rooms.value = [];
  roomsHistory.value = {};
  const config = await getConfig();
  const countdownSeconds = config!.countdown?.timerSeconds || 30;

  try {
    const rawRoomData = await roomDataStore.GetLast20();

    if (!rawRoomData || rawRoomData.length === 0) {
      console.warn("No room data received from the store.");
    } else {
      console.log(`Fetched ${rawRoomData.length} data points.`);
      processFetchedData(rawRoomData);

      if (hasRooms.value) {
        selectedRoom.value = rooms.value[0];
        setCards();
        initializeCharts();
        console.log("Default room selected:", selectedRoom.value.roomId);
      } else {
        console.log("No displayable rooms after processing data.");
      }
    }

    startCountdown(countdownSeconds);

    setInterval(() => {
      if (selectedRoom.value && hasHistoryDataForSelectedRoom.value) {
        updateChartsWithNewData();
        console.log("Charts updated (interval refresh)");
      }
    }, countdownSeconds * 1000);
  } catch (error) {
    console.error("Failed to fetch or process initial room data:", error);
    rooms.value = [];
    roomsHistory.value = {};
  } finally {
    loadingStore.setLoading(false);
    console.log("Initial data fetch attempt complete.");
  }
});

// ----- WebSocket Subscription -----

function subscribeToRoom(roomId: string) {
  const newEndpoint = `/RoomDatas/ws/${roomId}`;

  unsubscribeFromCurrentRoom();

  currentWsCallback = (data: any) => {
    if (!data || typeof data !== "object") {
      console.warn("[WS] Invalid data received:", data);
      return;
    }

    if (Array.isArray(data)) {
      if (data.length > 1) {
        data
          .sort(
            (a, b) => new Date(a.timeStamp).getTime() - new Date(b.timeStamp).getTime()
          )
          .reverse();
      }
      data = data[0];
      console.error("sethereuma", data);
    }

    // Validate required fields exist and are of correct type
    const requiredFields = {
      id: "number",
      humidity: "number",
      temperature: "number",
      pressure: "number",
      gas: "number",
      timeStamp: "string",
      roomId: "number",
      room: "object",
    };

    const isValid = Object.entries(requiredFields).every(
      ([field, type]) => field in data && typeof data[field] === type
    );

    if (!isValid) {
      console.warn("[WS] Data missing required fields or invalid types:", data);
      return;
    }

    const roomData = new RoomData(
      data.id,
      data.humidity,
      data.temperature,
      data.pressure,
      data.gas,
      data.timeStamp,
      data.roomId,
      data.room
    );

    handleWebSocketMessage(roomData);
  };

  console.log(`[WS] Subscribing to ${newEndpoint}`);
  socketService.subscribe(newEndpoint, currentWsCallback);
  currentWsEndpoint = newEndpoint;
}

function unsubscribeFromCurrentRoom() {
  if (currentWsEndpoint && currentWsCallback) {
    console.log(`[WS] Unsubscribing from ${currentWsEndpoint}`);
    socketService.unsubscribe(currentWsEndpoint, currentWsCallback);
    currentWsEndpoint = null;
    currentWsCallback = null;
  }
}

// ----- EVENT HANDLERS -----

function roomSelected(room: DisplayRoom) {
  // Check if its a different room before updating
  if (selectedRoom.value?.roomId !== room.roomId) {
    console.log("Room selected by user:", room.roomId);
    selectedRoom.value = room;
    // Update cards and charts when room changes (handled by watcher)
  }
}

// Watch for changes in selectedRoom to update UI
watch(
  selectedRoom,
  (newRoom, oldRoom) => {
    if (newRoom?.roomId !== oldRoom?.roomId) {
      console.log(`Selected room changed from ${oldRoom?.roomId} to ${newRoom?.roomId}`);

      // Unsubscribe happens inside subscribeToRoom, but calling it here for clarity
      unsubscribeFromCurrentRoom();

      setCards();
      initializeCharts();

      if (newRoom) {
        subscribeToRoom(newRoom.roomId);
      } else {
        unsubscribeFromCurrentRoom();
      }
    }
  },
  { immediate: false }
); // Don't run immediately, onMounted handles initial setup
</script>

<template>
  <div v-if="loadingStore.isLoading">
    <Loading class="mt-12" />
  </div>

  <!-- Fallback 1: No Rooms -->
  <NoRooms v-else-if="!hasRooms" />

  <!-- Main content -->
  <div v-else id="main-content">
    <RoomSelectorCard
      :latestFetch="latestFetch"
      :rooms="rooms"
      :selectedRoom="selectedRoom"
      :countdown="countdown"
      :isPingPending="isPingPending"
      @roomSelected="roomSelected"
      @pingRoom="handlePingRoom"
      v-if="!minimizedStore.isMinimized"
    />

    <!-- Statistic Cards -->
    <div v-if="!isBurning">
      <div
        v-if="selectedRoom"
        :class="[
          'mt-4 grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 items-center',
          { 'h-[90vh] flex justify-center items-center': minimizedStore.isMinimized },
        ]"
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
      <div v-if="selectedRoom && !minimizedStore.isMinimized" class="mt-4">
        <!-- Fallback 2: No History Data for the selected room -->
        <NoCharts v-if="!hasHistoryDataForSelectedRoom" :selectedRoom="selectedRoom" />

        <!-- Display Table and Charts -->
        <div v-else>
          <!-- Room Table -->
          <RoomTable
            :room-data="roomsHistory"
            :selected-room="selectedRoom"
            class="mt-4"
          />

          <ChartsSection :tabs="tabs" :charts="charts" :chart-titles="chartTitles" />
        </div>
      </div>
    </div>
    <div v-else>
      <Dialog
        v-model:visible="isBurning"
        modal
        header="🔥🔥🔥🔥 DER RAUM BRENNT 🔥🔥🔥🔥"
        :style="{ width: '75vw' }"
      >
        <Burning />
      </Dialog>
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

.minimized-layout {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
