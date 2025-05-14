import { defineStore } from "pinia";

export const useMinimizedStore = defineStore("Minimized", () => {
  const _isMinimized = ref(false);
  const isMinimized = computed(() => _isMinimized.value);

  function setMinimized(Minimized: boolean) {
    _isMinimized.value = Minimized;
  }

  return {
    setMinimized,
    isMinimized,
  }
});
