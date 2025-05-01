import { defineStore } from "pinia";
import { useToast } from "#imports";

export const useToastStore = defineStore("toast", () => {
  const toast = useToast();

  function setToast(severity: string, summary: string, detail: string) {
    toast.add({
      severity: severity,
      summary: summary,
      detail: detail,
      life: 3000,
    });
  }

  return {
    setToast,
  };
});