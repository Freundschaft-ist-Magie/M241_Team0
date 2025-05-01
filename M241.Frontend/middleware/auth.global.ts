import { useAuthStore } from '~/utils/stores/base/AuthStore';

export default defineNuxtRouteMiddleware((to) => {
  if (process.client) {
    const authStore = useAuthStore();

    if (!authStore.initialized) {
      authStore.init();
    }

    console.log('Middleware check:', {
      isAuthenticated: authStore.isAuthenticated,
      hasAccessToken: !!authStore.accessToken,
      initialized: authStore.initialized,
      path: to.path
    });

    // If user is trying to access login page but is already authenticated, redirect to home
    if (to.path === '/login' && authStore.isAuthenticated) {
      console.log('User already authenticated, redirecting to home');
      return navigateTo('/');
    }

    // If user is trying to access protected route but is not authenticated, redirect to login
    if (to.path !== '/login' && !authStore.isAuthenticated) {
      console.log('User not authenticated, redirecting to login');
      return navigateTo('/login');
    }
  }
});
