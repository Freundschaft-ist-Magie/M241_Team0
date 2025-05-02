<script lang="ts" setup>
import { usePageSettingsStore } from "@/utils/stores/base/PageSettingsStore";

const pageSettingsStore = usePageSettingsStore();
</script>

<template>
  <div
    :class="pageSettingsStore.isDarkMode ? 'bg-dark' : 'bg'"
    class="bg min-h-screen p-4"
  >
    <MainLayoutHeader />

    <slot />
  </div>
</template>

<style scoped>
.bg {
  background: #141a22;
  background: linear-gradient(305deg, #141a22 0%, #0060b9 0%, #afd8f5 100%);
}

.bg-dark {
  background: #141a22;
  background: linear-gradient(305deg, #141a22 0%, #003c74 0%, #2d3f4e 100%);
}
</style>
