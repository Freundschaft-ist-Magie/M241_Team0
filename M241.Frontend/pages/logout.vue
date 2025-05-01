<script setup>
import { useAuthStore } from "~/utils/stores/base/AuthStore";

const authStore = useAuthStore();
const router = useRouter();

onMounted(() => {
  authStore.clearUser();
  router.push("/login");
});
</script>

<template>
  <div class="flex items-center justify-center h-screen">
    <p>Logging out...</p>
  </div>
</template>
