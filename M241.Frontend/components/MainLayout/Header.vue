<script lang="ts" setup>
import { usePageSettingsStore } from "~/utils/stores/base/PageSettingsStore";
import { useMinimizedStore } from "~/utils/stores/base/MinimizedStore";
import { get } from "~/utils/services/base/ApiService";

const pageSettingsStore = usePageSettingsStore();

const navLinksPanel = ref();
const serverStatusPopover = ref();
const isServerOnline = ref(false);
const userData = ref({
  name: "Max Mustermann",
  image: "https://avatars.githubusercontent.com/u/143788063?s=200&v=4",
});

const minimizedStore = useMinimizedStore();

const toggleNavLinksPanel = (event: Event) => {
  navLinksPanel.value.toggle(event);
};

const toggleServerStatusPopover = (event: Event) => {
  serverStatusPopover.value.toggle(event);
};

const panelLinks = computed(() => {
  return [
    {
      id: "darkmode",
      text: pageSettingsStore.isDarkMode ? "Light Mode" : "Dark Mode",
      icon: pageSettingsStore.isDarkMode ? "weather-sunny" : "weather-night",
      action: () => pageSettingsStore.toggleDarkMode(),
      type: "button" as const,
    },
    {
      id: "admindashboard",
      to: "/admin/dashboard",
      text: "Admin Dashboard",
      icon: "view-dashboard-edit",
      type: "link" as const,
    },
    {
      id: "logout",
      to: "/logout",
      text: "Logout",
      icon: "logout",
      type: "link" as const,
    },
  ];
});

const checkServerHealth = async () => {
  try {
    const response = await get("healthz");
    if (response.ok) {
      const text = await response.text();
      isServerOnline.value = text.toLowerCase().includes("healthy");
    } else {
      isServerOnline.value = false;
    }
  } catch (error) {
    isServerOnline.value = false;
  }
};

onMounted(() => {
  checkServerHealth();

  setInterval(() => {
    checkServerHealth();
    console.log("Server health check executed");
  }, 2 * 60 * 1000);
});

const showMaxButton = ref(false);

// Überwache showMaxButton und setze es nach 2 Sekunden auf false, falls es true ist
watch(showMaxButton, (newValue) => {
  if (newValue) {
    setTimeout(() => {
      showMaxButton.value = false;
    }, 2000); // 2 Sekunden
  }
});
</script>

<template>
  <div
    class="w-full h-20 fixed top-0 left-0 z-50 flex justify-center items-center"
    v-if="minimizedStore.isMinimized"
    @mousemove="showMaxButton = true"
    @mouseleave="showMaxButton = false"
  >
    <transition name="fade-in-top">
      <Button
        v-if="showMaxButton"
        @click="minimizedStore.setMinimized(false)"
        class="absolute top-0 right-0 m-2"
        icon="pi pi-window-maximize"
      />
    </transition>
  </div>
  <div
    class="bg-gray2 p-4 flex justify-between items-center rounded-md shadow-md shadow-black/40 dark:bg-darkNeutral1 dark:text-darkNeutral2"
    v-else
  >
    <Button
      severity="secondary"
      class="p-0! m-0! bg-transparent! border-0!"
      aria-haspopup="true"
      aria-controls="navLinksPanel"
    >
      <NuxtLink to="/" class="flex justify-center items-center gap-4">
        <Icon
          name="mdi:air-conditioner"
          class="text-4xl text-primary2 dark:text-darkPrimary1"
        />
        <span
          class="hidden sm:block text-2xl font-bold text-primary2 dark:text-darkPrimary1"
        >
          AirCheck
        </span>
      </NuxtLink>
    </Button>

    <div class="flex items-center gap-4 space-x-2 text-black dark:text-darkNeutral2">
      <div
        class="relative flex gap-2 items-center cursor-pointer"
        @click="toggleServerStatusPopover"
      >
        <svg
          :class="isServerOnline ? 'text-green-500' : 'text-red-500'"
          class="w-3 h-3"
          viewBox="0 0 16 16"
          fill="currentColor"
          xmlns="http://www.w3.org/2000/svg"
        >
          <circle cx="8" cy="8" r="8" />
        </svg>
        <span class="text-base">{{ isServerOnline ? "Online" : "Offline" }}</span>
        <i class="pi pi-question-circle"></i>
      </div>
      <Popover ref="serverStatusPopover">
        <span class="text-sm text-gray-500 dark:text-darkSecondary2">
          Zeigt an, ob der Server erreichbar ist oder nicht. Ist der Server nicht
          erreichbar, werden die Daten nicht aktualisiert.
        </span>
      </Popover>

      <Button
        icon="pi pi-sort-alt"
        class="p-0! m-0! bg-transparent! border-0!"
        severity="secondary"
        @click="minimizedStore.setMinimized(true)"
      ></Button>

      <div class="flex gap-2 items-center">
        <Button
          severity="secondary"
          class="p-0! m-0! bg-transparent! border-0! flex items-center justify-center text-black dark:text-darkNeutral2"
          @click="toggleNavLinksPanel"
          aria-haspopup="true"
          aria-controls="navLinksPanel"
        >
          <Icon name="mdi:menu" class="w-6 h-6 p-5" />
        </Button>

        <OverlayPanel ref="navLinksPanel" id="navLinksPanel">
          <div class="flex flex-col gap-1 p-2 min-w-[200px]">
            <!-- User Info Header -->
            <div
              class="flex items-center gap-3 p-2 border-b border-gray-200 dark:border-gray-700 mb-2"
            >
              <img
                class="w-10 h-10 rounded-full"
                :src="userData.image"
                :alt="userData.name"
              />
              <span class="font-semibold">{{ userData.name }}</span>
            </div>

            <!-- Dynamic Links/Actions -->
            <template v-for="item in panelLinks" :key="item.id">
              <button
                v-if="item.type === 'button'"
                @click="item.action"
                class="flex items-center gap-3 text-left hover:bg-gray-100 dark:hover:bg-darkAccent2 px-3 py-2 rounded-md w-full text-sm"
              >
                <Icon :name="`mdi:${item.icon}`" class="w-5 h-5" />
                <span>{{ item.text }}</span>
              </button>
              <NuxtLink
                v-else-if="item.type === 'link'"
                :to="item.to!"
                class="flex items-center gap-3 text-left hover:bg-gray-100 dark:hover:bg-darkAccent2 px-3 py-2 rounded-md text-sm"
                @click="navLinksPanel.hide()"
              >
                <Icon :name="`mdi:${item.icon}`" class="w-5 h-5" />
                <span>{{ item.text }}</span>
              </NuxtLink>
            </template>
          </div>
        </OverlayPanel>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Transition für Fade-in + Bewegung nach unten */
.fade-in-top-enter-active,
.fade-in-top-leave-active {
  transition: transform 0.5s ease, opacity 0.5s ease;
}

.fade-in-top-enter-from {
  opacity: 0;
  transform: translateY(-20px); /* Startpunkt: Außerhalb der Ansicht */
}

.fade-in-top-leave-to {
  opacity: 0;
  transform: translateY(-20px); /* Zielpunkt beim Ausblenden: Zurück nach oben */
}
</style>
