import { useAuthStore } from '~/utils/stores/base/AuthStore';

export default defineNuxtRouteMiddleware((to) => {
  if (to.path === '/login') return;

  const authStore = useAuthStore();

  // If not authenticated, redirect to login page
  if (!authStore.isAuthenticated) {
    return navigateTo('/login');
  }
});
