import { defineStore } from "pinia";
import { useConfirm } from "#imports";

export const useConfirmStore = defineStore("confirm", () => {
  const confirm = useConfirm();

  function setConfirm(
    message: string,
    header: string,
    icon: string,
    accept: () => void,
    reject: () => void,
    acceptLabel = "Yes",
    rejectLabel = "No",
    acceptSeverity = "primary",
    rejectSeverity = "secondary",
  ) {
    confirm.require({
      message,
      header,
      icon,
      rejectProps: {
        label: rejectLabel,
        severity: rejectSeverity,
        outlined: true,
      },
      acceptProps: {
        label: acceptLabel,
        severity: acceptSeverity,
      },
      accept,
      reject,
    });
  }

  return {
    setConfirm,
  };
});
