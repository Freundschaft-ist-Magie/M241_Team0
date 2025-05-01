import { defineStore } from 'pinia';

export const usePageSettingsStore = defineStore("pageSettings", () => {
  function initialize() {
    console.log("init");
  }

  return { initialize };
});