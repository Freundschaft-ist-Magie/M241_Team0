<script setup lang="ts">
import Notification from "~/models/Notification";
import { useNotificationStore } from "~/utils/stores/NotificationStore";
import { formatRelativeTime, getNotificationGroup } from "~/utils/helper/DateUtils";

// --- Stores ---
const notificationStore = useNotificationStore();

// --- Reactive State ---
const filterOptions = [
  { label: "Gelesen markieren", value: "read" },
  { label: "Ungelesen markieren", value: "unread" },
  { label: "Löschen", value: "delete" },
];

const showFilterOptions = [
  { label: "Alle", value: "all" },
  { label: "Ungelesen", value: "unread" },
  { label: "Gelesen", value: "read" },
];

const selectedBulkAction = ref<string | null>(null);
const selectedShowFilter = ref<string>("all");
const selectedNotificationIds = ref<number[]>([]);

const notifications = notificationStore.notifications;

// --- Computed Properties ---

const filteredNotifications = computed(() => {
  return notifications.value.filter((n) => {
    if (selectedShowFilter.value === "all") return true;
    if (selectedShowFilter.value === "unread") return !n.read;
    if (selectedShowFilter.value === "read") return n.read;
    return false;
  });
});

const groupedNotifications = computed(() => {
  const groups: Record<string, Notification[]> = {};
  const groupOrder = ["Heute", "Gestern", "Diese Woche", "Diesen Monat", "Älter"];

  // Ensure all potential group keys exist for ordering, even if empty initially
  groupOrder.forEach((groupName) => (groups[groupName] = []));

  filteredNotifications.value.forEach((n) => {
    const groupName = getNotificationGroup(n.timestamp);
    if (groups[groupName]) {
      // Should always exist now
      groups[groupName].push(n);
    } else {
      console.warn(`Notification ${n.id} mapped to unexpected group: ${groupName}`);
      if (!groups["Älter"]) groups["Älter"] = []; // Fallback safety
      groups["Älter"].push(n); // Put into Älter as fallback
    }
  });

  // Filter out empty groups *after* processing all notifications
  const result: Record<string, Notification[]> = {};
  for (const groupName of groupOrder) {
    // Iterate in desired order
    if (groups[groupName] && groups[groupName].length > 0) {
      result[groupName] = groups[groupName];
    }
  }
  return result;
});

// Get Ids of currently visible notifications
const visibleNotificationIds = computed(() => {
  return filteredNotifications.value.map((n) => n.id);
});

// --- Master Checkbox v-model ---
const masterCheckboxModel = computed({
  get() {
    const visibleIds = visibleNotificationIds.value;
    return (
      visibleIds.length > 0 &&
      visibleIds.every((id) => selectedNotificationIds.value.includes(id))
    );
  },
  set(newValue: boolean) {
    const visibleIds = visibleNotificationIds.value;
    if (visibleIds.length === 0) return;

    // it works, don't touch it
    // use nextTick to check that DOM is updated before executing
    nextTick(() => {
      if (newValue) {
        // Use Sets for efficiency and potentially better reactivity triggering
        const selectedSet = new Set(selectedNotificationIds.value);
        visibleIds.forEach((id) => selectedSet.add(id));
        // Assign a new array reference
        selectedNotificationIds.value = Array.from(selectedSet);
      } else {
        // Deselect all visible
        const visibleSet = new Set(visibleIds);
        // Assign a new array reference
        selectedNotificationIds.value = selectedNotificationIds.value.filter(
          (id) => !visibleSet.has(id)
        );
      }
    });
  },
});

// Determines if the master checkbox should be INDETERMINATE
// (Remains the same, based on current selection state)
const isMasterCheckboxIndeterminate = computed(() => {
  const visibleIds = visibleNotificationIds.value;
  const selectedVisibleCount = visibleIds.filter((id) =>
    selectedNotificationIds.value.includes(id)
  ).length;
  // Indeterminate is if there are visible items, some are selected, but not all
  return (
    visibleIds.length > 0 &&
    selectedVisibleCount > 0 &&
    selectedVisibleCount < visibleIds.length
  );
});

// --- Methods ---
async function markAs(notification: Notification, isRead: boolean) {
  await notificationStore.UpdateRead(notification, isRead);
}

async function deleteNotification(notification: Notification) {
  await notificationStore.Delete(notification);
}

// --- Watchers ---
watch(selectedBulkAction, async (newValue) => {
  if (newValue && selectedNotificationIds.value.length > 0) {
    console.log(
      `Applying bulk action: ${newValue} to IDs: ${selectedNotificationIds.value.join(
        ", "
      )}`
    );

    const selectedNotifications = notifications.value.filter((n) =>
      selectedNotificationIds.value.includes(n.id)
    );

    if (newValue === "read" || newValue === "unread") {
      const isRead = newValue === "read";
      await notificationStore.UpdateMultipleRead(selectedNotifications, isRead);
    } else if (newValue === "delete") {
      await notificationStore.DeleteMultiple(selectedNotifications);
    }

    selectedNotificationIds.value = []; // Clear selection after bulk action
    selectedBulkAction.value = null;
  } else if (newValue) {
    selectedBulkAction.value = null;
    console.warn("Bulk action selected, but no notifications were checked.");
  }
});

// Watch for changes in filters or notifications to potentially clear selection
watch(
  [filteredNotifications],
  () => {
    const visibleIdsSet = new Set(visibleNotificationIds.value);
    selectedNotificationIds.value = selectedNotificationIds.value.filter((id) =>
      visibleIdsSet.has(id)
    );
  },
  { deep: true }
); // Use deep if notifications array content changes often internally
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex items-center gap-2 mt-4">
      <i class="pi pi-bell text-2xl text-primary"></i>
      <h1 class="text-2xl md:text-3xl text-primary font-bold">Benachrichtigungen</h1>
    </div>

    <Message severity="info" :closable="false" class="mb-4">
      <i class="pi pi-info-circle flex-shrink-0 mr-2"></i>
      Alle Benachrichtigungen, die älter als ein Monat sind, werden automatisch vom Server
      gelöscht.
    </Message>

    <div
      class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 p-4 bg-gray-50 border rounded-md"
    >
      <div class="flex items-center gap-2">
        <!-- Master Checkbox -->
        <Checkbox
          v-model="masterCheckboxModel"
          :binary="true"
          :indeterminate="isMasterCheckboxIndeterminate"
          inputId="selectAll"
          :disabled="visibleNotificationIds.length === 0"
        />
        <label
          for="selectAll"
          class="text-sm ml-1 cursor-pointer select-none"
          :class="{ 'text-gray-400': visibleNotificationIds.length === 0 }"
        >
          Alle Sichtbaren auswählen ({{ selectedNotificationIds.length }} ausgewählt)
        </label>
      </div>
      <div class="flex flex-col sm:flex-row gap-2 w-full sm:w-auto">
        <Select
          v-model="selectedBulkAction"
          :options="filterOptions"
          optionLabel="label"
          optionValue="value"
          placeholder="Mehrfachaktion auswählen"
          class="w-full sm:w-auto md:min-w-[200px] p-2"
          :disabled="selectedNotificationIds.length === 0"
        />
        <Select
          v-model="selectedShowFilter"
          :options="showFilterOptions"
          optionLabel="label"
          optionValue="value"
          placeholder="Anzeigen"
          class="w-full sm:w-auto md:min-w-[150px] p-2"
        />
      </div>
    </div>

    <div v-if="filteredNotifications.length > 0">
      <div
        v-for="(groupNotifications, groupName) in groupedNotifications"
        :key="groupName"
        class="mb-6"
      >
        <h2 class="text-xl font-semibold mb-4 text-gray-700">
          {{ groupName }}
        </h2>
        <div class="flex flex-col gap-3">
          <div
            v-for="n in groupNotifications"
            :key="n.id"
            class="flex gap-3 items-center bg-white p-4 border border-gray-200 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-200"
            :class="{ 'opacity-70 bg-gray-50': n.read }"
          >
            <div class="flex-shrink-0">
              <Checkbox
                v-model="selectedNotificationIds"
                :value="n.id"
                :inputId="`notif-${n.id}`"
              />
            </div>

            <div
              v-if="n.image"
              class="flex-shrink-0 w-12 h-12 flex items-center justify-center bg-gray-100 rounded-full overflow-hidden"
            >
              <img
                :src="n.image"
                alt="notification icon"
                class="max-h-full max-w-full object-cover"
              />
            </div>
            <div
              v-else
              class="flex-shrink-0 w-12 h-12 flex items-center justify-center bg-gray-200 rounded-full text-gray-500"
            >
              <i class="pi pi-bell"></i>
            </div>

            <div class="flex flex-col flex-grow min-w-0">
              <span class="text-gray-800 break-words">{{ n.text }}</span>
              <span class="text-xs text-gray-500 mt-1">{{
                formatRelativeTime(n.timestamp)
              }}</span>
            </div>

            <!-- Right Actions -->
            <div class="flex flex-shrink-0 items-center gap-1 sm:gap-2 ml-auto pl-2">
              <!-- Reduced gap slightly -->
              <span class="text-sm text-gray-500 hidden sm:inline">{{
                n.read ? "Gelesen" : "Ungelesen"
              }}</span>

              <!-- Mark as Read / Unread Button -->
              <Button
                v-if="!n.read"
                icon="pi pi-eye"
                class="p-button-rounded p-button-text p-button-sm text-gray-600 hover:text-primary"
                v-tooltip.top="'Als gelesen markieren'"
                @click="markAs(n, true)"
                aria-label="Als gelesen markieren"
              />
              <Button
                v-if="n.read"
                icon="pi pi-eye-slash"
                class="p-button-rounded p-button-text p-button-sm text-gray-600 hover:text-primary"
                v-tooltip.top="'Als ungelesen markieren'"
                @click="markAs(n, false)"
                aria-label="Als ungelesen markieren"
              />

              <Button
                icon="pi pi-trash"
                class="p-button-rounded p-button-text p-button-danger p-button-sm"
                v-tooltip.top="'Löschen'"
                @click="deleteNotification(n)"
                aria-label="Löschen"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center text-gray-500 py-10">
      Keine Benachrichtigungen zum Anzeigen vorhanden.
    </div>
  </div>
</template>

<style scoped>
.min-w-0 {
  min-width: 0;
}
</style>
