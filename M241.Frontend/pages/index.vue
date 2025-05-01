<script lang="ts" setup>
import { useLoadingStore } from "@/utils/stores/base/LoadingStore";
import { useRoomStore } from "~/utils/stores/RoomStore";
import RoomsService from "~/utils/services/RoomsService";
import Room from "~/models/Room";
import GlobalHelper from "~/utils/helper/GlobalHelper";
import StatisticCardObj from "~/models/StatisticCardObj";
import ChartData from "~/models/ChartData";
import ChartOptions from "~/models/ChartOptions";
import { getConfig } from "~/utils/helper/ConfigLoader";

const loadingStore = useLoadingStore();
const roomsStore = useRoomStore();

const latestFetch = ref(new Date());
const selectedRoom = ref<Room | null>(null);
const countdown = ref(0);
const rooms = ref<Room[]>([]);
const roomsHistory = ref<
  Record<
    string,
    {
      timeStamp: string;
      temperature: number;
      humidity: number;
      pressure: number;
      airQuality: number;
    }[]
  >
>({});
const cards = ref<StatisticCardObj[]>([]);
const charts = ref<{ data: ChartData; options: ChartOptions }[]>([]);
const historyCharts = ref<{ data: ChartData; options: ChartOptions }[]>([]);
const chartTitles = ref([
  "Temperatur in den letzten 24 h",
  "Luftfeuchtigkeit in den letzten 24 h",
  "Luftqualität Level in den letzten 24 h",
  "Luftdruck in den letzten 24h",
]);
const monthChartTitles = ref([
  "Temperatur in den letzten 30 Tagen",
  "Luftfeuchtigkeit in den letzten 30 Tagen",
  "Luftqualität in den letzten 30 Tagen",
  "Luftdruck in den letzten 30 Tagen",
]);
const tabs = ref([
  { title: "Temperatur", value: "0", dayChart: 0, monthChart: 0 },
  { title: "Luftfeuchtigkeit", value: "1", dayChart: 1, monthChart: 1 },
  { title: "CO₂ Level", value: "2", dayChart: 2, monthChart: 2 },
  { title: "Luftdruck", value: "3", dayChart: 3, monthChart: 3 },
]);

loadingStore.setLoading(true);

const hasRooms = computed(() => rooms.value.length > 0);

const hasHistoryDataForSelectedRoom = computed(() => {
  if (!selectedRoom.value) return false;

  const history = roomsHistory.value[selectedRoom.value.roomId];
  return history && history.length > 0;
});

function startCountdown(countdownSeconds: number) {
  countdown.value = countdownSeconds;
  setInterval(() => {
    countdown.value--;
    if (countdown.value <= 0) {
      countdown.value = countdownSeconds;
    }
  }, 1000);
}

function setCards() {
  cards.value = []; // Clear existing cards first to avoid duplicates if called multiple times
  if (!selectedRoom.value) return; // Prevent errors if no room is selected

  const cardTemperature = GlobalHelper.MapTemperature(selectedRoom.value.temperature);
  const cardHumidity = GlobalHelper.MapHumidity(selectedRoom.value.humidity);
  const cardAirQuality = GlobalHelper.MapAirQuality(selectedRoom.value.gas);
  const cardPressure = GlobalHelper.MapPressure(selectedRoom.value.pressure);

  if (cardTemperature.value !== undefined) {
    // Vorhandene Karten ersetzen statt hinzufügen
    cards.value = [cardTemperature, cardHumidity, cardAirQuality, cardPressure];
  }
}

function setCharts() {
  charts.value = [];
  historyCharts.value = []; // Also reset month charts if they exist

  // 1️⃣ History des selektierten Raums auslesen (falls vorhanden)
  const roomId = selectedRoom.value?.roomId;
  // Use nullish coalescing for safety, default to empty array if no history or no room selected
  const historyForSelected = roomId != null ? roomsHistory.value[roomId] ?? [] : [];

  // Proceed only if there is history data
  if (historyForSelected.length > 0) {
    // 2️⃣ Diagrammdaten nur für den selektierten Raum erstellen
    const temperatureData = GlobalHelper.MapChartDataTemperature(historyForSelected);
    const humidityData = GlobalHelper.MapChartDataHumidity(historyForSelected);
    const airQualityData = GlobalHelper.MapChartDataAirQuality(historyForSelected);
    const pressureData = GlobalHelper.MapChartDataPressure(historyForSelected);

    // 3️⃣ Options initialisieren
    const chartOptions = new ChartOptions();

    // 4️⃣ Charts-Array befüllen
    charts.value.push(
      { data: temperatureData, options: chartOptions },
      { data: humidityData, options: chartOptions },
      { data: airQualityData, options: chartOptions },
      { data: pressureData, options: chartOptions }
    );
  }
}

onMounted(async () => {
  loadingStore.setLoading(true);
  const config = await getConfig();
  const countdownSeconds = config!.countdown?.timerSeconds || 30; // fallback to 30 if missing

  const webSocket = new WebSocket(
    `ws://${import.meta.env.VITE_API_URL}/api/roomDatas/ws`
  );

  webSocket.onmessage = (event) => {
    try {
      const data = JSON.parse(event.data);
      latestFetch.value = new Date();
      let needsUIClear = false;
      let selectedRoomExists = false;

      data.forEach((roomData: any) => {
        const existingRoomIndex = rooms.value.findIndex(
          (room) => room.roomId === roomData.roomId
        );
        if (existingRoomIndex !== -1) {
          // Update existing room (more efficient to update in place)
          Object.assign(rooms.value[existingRoomIndex], {
            temperature: roomData.temperature,
            humidity: roomData.humidity,
            pressure: roomData.pressure,
            gas: roomData.gas,
            timeStamp: roomData.timeStamp,
          });
          if (
            selectedRoom.value &&
            rooms.value[existingRoomIndex].roomId === selectedRoom.value.roomId
          ) {
            selectedRoomExists = true;
          }
        } else {
          rooms.value.push({
            roomId: roomData.roomId,
            temperature: roomData.temperature,
            humidity: roomData.humidity,
            pressure: roomData.pressure,
            gas: roomData.gas,
            timeStamp: roomData.timeStamp,
          });
        }

        const id = roomData.roomId;
        const historyForRoom = roomsHistory.value[id] ?? [];
        const lastEntry = historyForRoom[historyForRoom.length - 1];
        if (!lastEntry || new Date(roomData.timeStamp) > new Date(lastEntry.timeStamp)) {
          // Keep history size manageable if desired (e.g., last 100 entries)
          // if (historyForRoom.length > 100) {
          //   historyForRoom.shift();
          // }
          historyForRoom.push({
            timeStamp: roomData.timeStamp,
            temperature: roomData.temperature,
            humidity: roomData.humidity,
            pressure: roomData.pressure,
            airQuality: roomData.gas,
          });
        }
        roomsHistory.value[id] = historyForRoom;
      });

      // If rooms were received, but the previously selected one wasn't among them
      if (
        selectedRoom.value &&
        !data.some((rd: any) => rd.roomId === selectedRoom.value?.roomId)
      ) {
        // Option 1: Select the first available room if any exist
        if (rooms.value.length > 0) {
          selectedRoom.value = rooms.value[0];
          console.log(
            "Selected room disappeared, auto-selecting first available:",
            selectedRoom.value.roomId
          );
          // Setup specific WS for the new room
          setupRoomSpecificWebSocket(selectedRoom.value);
        } else {
          // Option 2: No rooms left, clear selection
          selectedRoom.value = null;
          needsUIClear = true;
          console.log("Selected room disappeared, no other rooms available.");
        }
      }
      // If no room was selected initially, or after clearing, select the first one if available
      if (!selectedRoom.value && rooms.value.length > 0) {
        selectedRoom.value = rooms.value[0];
        console.log(
          "No room selected, auto-selecting first available:",
          selectedRoom.value.roomId
        );
        // Setup specific WS for the newly selected room
        setupRoomSpecificWebSocket(selectedRoom.value);
      } else if (rooms.value.length === 0) {
        selectedRoom.value = null;
        needsUIClear = true;
        console.log("Update resulted in zero rooms.");
      }

      if (selectedRoom.value) {
        setCards();
        if (charts.value.length === 0) {
          // Update charts if they haven't been set yet
          setCharts();
        }
      } else if (needsUIClear) {
        cards.value = [];
        charts.value = [];
        historyCharts.value = [];
      }
    } catch (e) {
      console.error("Error processing WebSocket message:", e);
    } finally {
      if (loadingStore.isLoading) {
        loadingStore.setLoading(false);
      }
    }
  };

  webSocket.onerror = (error) => {
    console.error("WebSocket Error:", error);
    loadingStore.setLoading(false);
  };
  webSocket.onclose = () => {
    console.warn("WebSocket connection closed.");
    // Decide if UI should be cleared or state maintained
    // loadingStore.setLoading(false);
  };

  // Start countdown timer (only in browser; not during SSR)
  if (process.client) {
    startCountdown(countdownSeconds);

    // refresh charts every 30 seconds
    setInterval(() => {
      if (selectedRoom.value && hasHistoryDataForSelectedRoom.value) {
        setCharts();
        console.log("🔄 Charts updated (interval refresh)");
      }
    }, countdownSeconds * 1000); // convert to milliseconds
  }
});

function setupRoomSpecificWebSocket(room: Room | null) {
  if (!room) return;

  console.log(`Setting up specific WS for room: ${room.roomId}`);
  const ws = new WebSocket(
    `ws://${import.meta.env.VITE_API_URL}/api/roomDatas/ws/${room.roomId}`
  );

  ws.onmessage = (event) => {
    try {
      const data = JSON.parse(event.data);
      console.log("🔥 Received specific data for room:", data.roomId, data);

      if (data.isBurning) {
        console.warn("🔥🔥🔥 ALARM: Dieser Raum brennt! 🔥🔥🔥", room.roomId);
        triggerTheInferno();
      }
    } catch (e) {
      console.error("Failed to parse specific room WS message:", e);
    }
  };

  ws.onerror = (error) =>
    console.error(`WebSocket Error for room ${room.roomId}:`, error);
  ws.onclose = (event) =>
    console.warn(
      `WebSocket connection closed for room ${room.roomId}:`,
      event.code,
      event.reason
    );
}

function roomSelected(room: Room) {
  if (selectedRoom.value?.roomId !== room.roomId) {
    console.log("Set new room", selectedRoom.value?.roomId ?? "None", "->", room.roomId);
    selectedRoom.value = room;
    setCards();
    setCharts();
    setupRoomSpecificWebSocket(room);
  }
}

function triggerTheInferno() {
  alert("🔥 Achtung: Der Raum " + selectedRoom.value?.roomId + " steht in Flammen!");
  launchFlyingFlames(15);

  const mainContent = document.getElementById("main-content");
  if (mainContent) {
    mainContent.classList.add("inferno-effect");
    setTimeout(() => {
      mainContent.classList.remove("inferno-effect");
    }, 8000);
  }

  const statCards = document.querySelectorAll(".stat-card");
  statCards.forEach((card) => {
    (card as HTMLElement).classList.add("inferno-effect");
    setTimeout(() => {
      (card as HTMLElement).classList.remove("inferno-effect");
    }, 8000);
  });
}

function launchFlyingFlames(count: number) {
  console.log(`Launching ${count} flames...`);
  for (let i = 0; i < count; i++) {
    const flame = document.createElement("div");
    flame.classList.add("flame");
    flame.style.left = `${Math.random() * 10 - 10}vw`;
    flame.style.top = `${Math.random() * 80 + 10}vh`;
    flame.style.animationDuration = `${Math.random() * 2 + 3}s`;
    flame.style.animationDelay = `${Math.random() * 1}s`;
    document.body.appendChild(flame);
    setTimeout(
      () => flame.remove(),
      parseFloat(flame.style.animationDuration) * 1000 +
        parseFloat(flame.style.animationDelay) * 1000
    );
  }
}
</script>

<template>
  <div v-if="loadingStore.isLoading">
    <Loading class="mt-12" />
  </div>

  <!-- Fallback 1: No Rooms -->
  <div
    v-else-if="!hasRooms"
    class="mt-10 p-6 border border-dashed border-gray-400 rounded-lg bg-gray-50 text-center"
  >
    <h2 class="text-xl font-semibold text-gray-700 mb-2">Keine Räume verfügbar</h2>
    <p class="text-gray-500">
      Es wurden keine Raumdaten empfangen. Bitte überprüfen Sie die Sensoren und die
      Serververbindung.
    </p>
  </div>

  <!-- Main content -->
  <div v-else id="main-content">
    <RoomSelectorCard
      :latestFetch="latestFetch"
      :rooms="rooms"
      :selectedRoom="selectedRoom"
      :countdown="countdown"
      @roomSelected="roomSelected"
    />

    <div
      v-if="selectedRoom"
      class="mt-4 grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4 items-center"
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

    <div
      v-else
      class="mt-4 p-4 bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-300 rounded-md text-center"
    >
      Bitte wählen Sie oben einen Raum aus, um Details anzuzeigen.
    </div>

    <div v-if="selectedRoom" class="mt-4">
      <!-- Fallback 2: No History Data -->
      <div
        v-if="!hasHistoryDataForSelectedRoom"
        class="p-4 border border-dashed border-yellow-400 rounded-lg bg-yellow-50 dark:bg-yellow-900/50 text-center"
      >
        <p class="text-yellow-700 dark:text-yellow-300">
          Für den Raum "{{ selectedRoom.roomId }}" liegen derzeit keine Verlaufsdaten vor.
        </p>
        <p class="text-yellow-600 dark:text-yellow-400 text-sm mt-1">
          Diagramme und Tabelle werden angezeigt, sobald Daten empfangen werden.
        </p>
      </div>

      <div v-else>
        <RoomTable :room-data="roomsHistory" :selected-room="selectedRoom" class="mt-4" />

        <!-- Mobile Tabs -->
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
                  <StatisticDiagram
                    v-if="charts[tab.dayChart]"
                    :title="chartTitles[tab.dayChart]"
                    :chartData="charts[tab.dayChart].data"
                    :chartOptions="charts[tab.dayChart].options"
                    chartType="line"
                  />
                  <p v-else class="text-sm text-gray-500 text-center py-4">
                    Tages-Diagramm nicht verfügbar.
                  </p>
                  <!-- History/Month Chart Placeholder -->
                  <!--
                    <StatisticDiagram
                      v-if="historyCharts[tab.monthChart]"
                      :title="monthChartTitles[tab.monthChart]"
                      :chartData="historyCharts[tab.monthChart].data"
                      :chartOptions="historyCharts[tab.monthChart].options"
                      chartType="bar"
                    />
                     <p v-else class="text-sm text-gray-500 text-center py-4">Monats-Diagramm nicht verfügbar.</p>
                    -->
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
          <!-- Add a placeholder if charts array is empty but history exists (shouldn't normally happen with current logic, but safe) -->
          <p v-if="charts.length === 0" class="w-full text-center text-gray-500 py-4">
            Keine Diagramme zum Anzeigen.
          </p>
        </div>

        <!--
           <div v-if="historyCharts.length > 0" class="mt-4 hidden sm:flex flex-wrap gap-4">
              <StatisticDiagram
                v-for="(chart, index) in historyCharts"
                :key="'month-' + index"
                :title="monthChartTitles[index]"
                :chartData="chart.data"
                :chartOptions="chart.options"
                chartType="bar"
                class="flex-grow w-full md:w-[calc(50%-0.5rem)] min-w-[250px]"
              />
           </div>
           <div v-else class="mt-4 hidden sm:block text-center text-gray-500 py-4">
               Keine Monats-Diagramme verfügbar.
           </div>
           -->
      </div>
    </div>
  </div>
</template>

<style scoped>
@keyframes infernoShake {
  0% {
    transform: translate(1px, 1px) rotate(0deg);
  }

  10% {
    transform: translate(-1px, -2px) rotate(-1deg);
  }

  20% {
    transform: translate(-3px, 0px) rotate(1deg);
  }

  30% {
    transform: translate(3px, 2px) rotate(0deg);
  }

  40% {
    transform: translate(1px, -1px) rotate(1deg);
  }

  50% {
    transform: translate(-1px, 2px) rotate(-1deg);
  }

  60% {
    transform: translate(-3px, 1px) rotate(0deg);
  }

  70% {
    transform: translate(3px, 1px) rotate(-1deg);
  }

  80% {
    transform: translate(-1px, -1px) rotate(1deg);
  }

  90% {
    transform: translate(1px, 2px) rotate(0deg);
  }

  100% {
    transform: translate(1px, -2px) rotate(-1deg);
  }
}

@keyframes infernoGlow {
  0% {
    box-shadow: 0 0 10px red;
  }

  50% {
    box-shadow: 0 0 30px orange;
  }

  100% {
    box-shadow: 0 0 10px red;
  }
}

@keyframes flyAcross {
  0% {
    transform: translateX(-100px) translateY(0px) scale(0.5);
    opacity: 0;
  }

  10% {
    opacity: 1;
  }

  100% {
    transform: translateX(110vw) translateY(-200px) scale(1.5);
    opacity: 0;
  }
}

.flame {
  position: fixed;
  width: 40px;
  height: 40px;
  background-image: url("https://c.tenor.com/K3j9pwWlME0AAAAC/tenor.gif");
  /* Oder ein SVG oder Emoji 🔥 */
  background-size: cover;
  z-index: 9999;
  pointer-events: none;
  animation: flyAcross 4s linear forwards;
}

.inferno-effect {
  animation: infernoShake 0.5s infinite, infernoGlow 1s infinite;
  background: linear-gradient(45deg, #ff0000cc, #ff9900cc);
  color: white !important;
  border: 2px solid #ff9900;
}
</style>
