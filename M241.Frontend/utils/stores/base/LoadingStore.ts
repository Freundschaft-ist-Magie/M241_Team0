import { defineStore } from "pinia";

export const useLoadingStore = defineStore("loading", () => {
  const _isLoading = ref(false);
  const isLoading = computed(() => _isLoading.value);

  function setLoading(loading: boolean) {
    _isLoading.value = loading;
  }

  return {
    setLoading,
    isLoading,
  }
});
