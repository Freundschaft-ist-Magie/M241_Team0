import { defineStore } from 'pinia';
import StorageService from '@/utils/services/base/StorageService';

export const usePageSettingsStore = defineStore("pageSettings", () => {
  const _isDarkMode = ref(false);
  const isDarkMode = computed(() => _isDarkMode.value);

  function initialize() {
    initializeTheme();
  }

  function initializeTheme() {
    const savedTheme = StorageService.get("theme") || "light";
    
    if (savedTheme) {
      _isDarkMode.value = savedTheme === "dark";
    } else {
      const prefersDarkScheme = window.matchMedia("(prefers-color-scheme: dark)");
      _isDarkMode.value = prefersDarkScheme.matches;
      StorageService.set("theme", _isDarkMode.value ? "dark" : "light");
    }

    document.documentElement.classList.toggle("darkMode", _isDarkMode.value);
  }

  function toggleDarkMode() {
    _isDarkMode.value = !_isDarkMode.value;

    document.documentElement.classList.toggle("darkMode", _isDarkMode.value);
    StorageService.set("theme", _isDarkMode.value ? "dark" : "light");
  }

  return { isDarkMode, initialize, toggleDarkMode };
});