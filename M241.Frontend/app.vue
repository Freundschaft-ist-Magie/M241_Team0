<script lang="ts" setup>
import { useAuthStore } from "@/utils/stores/base/AuthStore";

const authStore = useAuthStore();

const layout = authStore.isAuthenticated ? "default" : "not-logged-in";
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
