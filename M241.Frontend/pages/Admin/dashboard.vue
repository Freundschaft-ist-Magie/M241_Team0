<script setup lang="ts">
import { useLoadingStore } from "~/utils/stores/base/LoadingStore";
import EditRoles from "~/components/editRoles.vue";
import { useRoleStore } from "~/utils/stores/RoleStore";
import { useUserStore } from "~/utils/stores/UserStore";

const loadingStore = useLoadingStore();

const roles = ref<
  {
    id: number;
    name: string;
    permissions: string;
  }[]
>([]);

const users = ref<
  {
    id: number;
    name: string;
    email: string;
    roles: {
      id: number;
      name: string;
      permissions: string;
    }[];
  }[]
>([]);

loadingStore.setLoading(true);

onMounted(async () => {
  roles.value = await useRoleStore().GetAll();
  users.value = await useUserStore().GetAll();
  loadingStore.setLoading(false);
});
</script>

<template>
  <div v-if="loadingStore.isLoading">
    <Loading class="mt-12" />
  </div>
  <div v-else class="container mx-auto flex flex-col gap-4 p-4">
    <div class="mt-6 border-b border-gray-300 pb-4 mb-4">
      <h1 class="text-2xl font-semibold mb-4">Admin Dashboard</h1>

      <Accordion :multiple="true" :activeIndex="[0]">
        <AccordionPanel value="0">
          <AccordionHeader>Rollen verwalten</AccordionHeader>
          <AccordionContent>
            <edit-roles :roles="roles" />
          </AccordionContent>
        </AccordionPanel>
        <AccordionPanel value="1">
          <AccordionHeader>Nutzer verwalten</AccordionHeader>
          <AccordionContent>
            <edit-users :roles="roles" :users="users" />
          </AccordionContent>
        </AccordionPanel>
        <!--
        <AccordionPanel value="2">
          <AccordionHeader>Räume verwalten</AccordionHeader>
          <AccordionContent>
            <edit-devices />
          </AccordionContent>
        </AccordionPanel>
        -->
      </Accordion>
    </div>
  </div>
</template>
