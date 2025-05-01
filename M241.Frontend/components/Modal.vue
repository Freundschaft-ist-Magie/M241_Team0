<script lang="ts" setup>
defineProps({
  title: {
    type: String,
    required: true,
  },
  visible: {
    type: Boolean,
    required: true,
  },
});

const emits = defineEmits(["cancel", "save", "update:visible"]);

function cancel() {
  emits("cancel");
}

function save() {
  emits("save");
}
</script>

<template>
  <Dialog
    :visible="visible"
    @update:visible="() => cancel()"
    modal
    :header="title"
    class="w-full sm:w-3/4 md:w-1/2 lg:w-1/3"
  >
    <slot />

    <div class="flex justify-end gap-2">
      <Button type="button" label="Cancel" severity="secondary" @click="cancel"></Button>
      <Button type="button" label="Save" @click="save"></Button>
    </div>
  </Dialog>
</template>
