import { useAuthStore } from '~/utils/stores/base/AuthStore';

export default defineNuxtPlugin((nuxtApp) => {
  // Initialize the auth store on client-side
  const authStore = useAuthStore();

  // Initialize immediately
  authStore.init();

  // Also check on page navigation
  nuxtApp.hook('page:start', () => {
    if (!authStore.initialized) {
      authStore.init();
    }

    console.log('Plugin page:start hook:', {
      isAuthenticated: authStore.isAuthenticated,
      hasAccessToken: !!authStore.accessToken
    });
  });

  // Add auth status to window for debugging
  if (process.dev) {
    window.$auth = authStore;
  }
});
