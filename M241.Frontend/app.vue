<script lang="ts" setup>
import { useAuthStore } from "@/utils/stores/base/AuthStore";
import { usePageSettingsStore } from "@/utils/stores/base/PageSettingsStore";

const authStore = useAuthStore();
const pageSettingsStore = usePageSettingsStore();

onMounted(() => {
  pageSettingsStore.initialize();
});
</script>

<template>
  <div>
    <NuxtLayout>
      <NuxtLoadingIndicator :color="'#fff'" />
      <Toast />
      <DynamicDialog />
      <ConfirmDialog></ConfirmDialog>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>
