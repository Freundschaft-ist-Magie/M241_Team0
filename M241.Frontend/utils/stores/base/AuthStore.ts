import { defineStore } from 'pinia';
import StorageService from '@/utils/services/base/StorageService';

export const useAuthStore = defineStore('auth', {
  state: () => {
    // Initialize with empty values
    return {
      role: '',
      accessToken: '',
      refreshToken: '',
      isAuthenticated: false,
      initialized: false
    };
  },
  actions: {
    // Initialize the store with values from localStorage
    init() {
      // Only run on client-side
      if (process.client) {
        const role = StorageService.get('role') || '';
        const accessToken = StorageService.get('accessToken') || '';
        const refreshToken = StorageService.get('refreshToken') || '';

        this.role = role;
        this.accessToken = accessToken;
        this.refreshToken = refreshToken;
        this.isAuthenticated = !!accessToken;
        this.initialized = true;

        console.log('Auth store initialized:', {
          isAuthenticated: this.isAuthenticated,
          accessToken: !!this.accessToken,
          valueLength: accessToken?.length || 0
        });
      }
    },
    setRole(role: string) {
      this.role = role;
      StorageService.set('role', role);
    },
    setTokens(accessToken: string, refreshToken: string) {
      this.accessToken = accessToken;
      this.refreshToken = refreshToken;
      this.isAuthenticated = true;
      this.initialized = true;

      StorageService.set('accessToken', accessToken);
      StorageService.set('refreshToken', refreshToken);

      console.log('Tokens set:', {
        isAuthenticated: this.isAuthenticated,
        accessToken: !!this.accessToken,
        valueLength: accessToken?.length || 0
      });
    },
    clearUser() {
      this.role = '';
      this.accessToken = '';
      this.refreshToken = '';
      this.isAuthenticated = false;

      StorageService.remove('role');
      StorageService.remove('accessToken');
      StorageService.remove('refreshToken');
    },
  },
  // Add a persist plugin for Pinia to make sure state persists
  persist: {
    enabled: true,
    strategies: [
      {
        key: 'auth',
        storage: process.client ? localStorage : null,
      },
    ],
  }
});
