<script lang="ts" setup>
import { useAuthStore } from "~/utils/stores/base/AuthStore";

const authStore = useAuthStore();
const navLinksPanel = ref();
const notifPanel = ref();
const serverStatusPopover = ref();
const isServerOnline = ref(false);
const userData = ref({
  // this will contain the current user's data
  name: "Max Mustermann",
  image: "https://avatars.githubusercontent.com/u/143788063?s=200&v=4",
});

const toggleNavLinksPanel = (event: Event) => {
  navLinksPanel.value.toggle(event);
};

const toggleNotifPanel = (event: Event) => {
  notifPanel.value.toggle(event);
};

const toggleServerStatusPopover = (event: Event) => {
  serverStatusPopover.value.toggle(event);
};

const panelLinks = computed(() => {
  const links = [
    { to: "/", text: "AirCheck Dashboard", icon: "view-dashboard" },
    { to: "/notifications", text: "Benachrichtigungen", icon: "bell" },
    { to: "/forecast/dashboard", text: "Prognose Dashboard", icon: "chart-line" },
    { to: "/user/settings", text: "Nutzer Einstellungen", icon: "cog" },
  ];

  // Only show admin dashboard link if user has admin role
  if (authStore.role === "admin") {
    links.push({ to: "/admin/dashboard", text: "Admin Dashboard", icon: "lock" });
  }

  links.push({ to: "/logout", text: "Logout", icon: "logout" });

  return links;
});

const checkServerHealth = async () => {
  try {
    const response = await fetch(`http://${import.meta.env.VITE_API_URL}/healthz`);
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
</script>

<template>
  <div
    class="bg-gray2 p-4 flex justify-between items-center rounded-md shadow-md shadow-black/40"
  >
    <Button
      severity="secondary"
      class="p-0! m-0! bg-transparent! border-0!"
      aria-haspopup="true"
      aria-controls="navLinksPanel"
    >
      <NuxtLink to="/" class="flex justify-center items-center gap-4">
        <Icon name="mdi:air-conditioner" class="text-4xl text-primary2" />
        <span class="hidden sm:block text-2xl font-bold text-primary2"> AirCheck </span>
      </NuxtLink>
    </Button>

    <div class="flex items-center gap-4 space-x-2 text-black">
      <div class="relative flex gap-2 items-center" @click="toggleServerStatusPopover">
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
        <span class="text-sm text-gray-500">
          Zeigt an, ob der Server erreichbar ist oder nicht. Ist der Server nicht
          erreichbar, werden die Daten nicht aktualisiert.
        </span>
      </Popover>

      <div class="flex gap-2 items-center">
        <!-- <Button
          severity="secondary"
          class="p-0! m-0! bg-transparent! border-0!"
          @click="toggleNavLinksPanel"
          aria-haspopup="true"
          aria-controls="navLinksPanel"
        >
          <img
            class="w-10 h-10 rounded-full"
            :src="userData.image"
            :alt="userData.name"
          />
          <p class="hidden sm:block">{{ userData.name }}</p>
        </Button>

        <OverlayPanel ref="navLinksPanel" id="navLinksPanel">
          <div class="flex flex-col gap-2 p-2">
            <NuxtLink
              v-for="link in panelLinks"
              :key="link.to"
              :to="link.to"
              class="flex items-center gap-2 text-left hover:bg-gray1/60 px-4 py-2 rounded-md"
            >
              <Icon :name="`mdi-light:${link.icon}`" class="w-5 h-5" />
              <span>{{ link.text }}</span>
            </NuxtLink>
          </div>
        </OverlayPanel> -->
        <Button
          severity="secondary"
          class="p-0! m-0! bg-transparent! border-0!"
          @click="toggleNavLinksPanel"
          aria-haspopup="true"
          aria-controls="navLinksPanel"
        >
          <NuxtLink
            to="logout"
            class="flex items-center gap-2 text-left hover:bg-gray1/60 px-4 py-2 rounded-md"
          >
            <Icon name="mdi-light:logout" class="w-5 h-5" />
            <span>Logout</span>
          </NuxtLink>
        </Button>
      </div>
    </div>
  </div>
</template>
