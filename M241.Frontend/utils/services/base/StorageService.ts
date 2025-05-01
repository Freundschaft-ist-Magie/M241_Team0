import { useNuxtApp } from "#app";

class StorageService {
  // Check if we're on client side
  private isClient(): boolean {
    return process.client && typeof window !== "undefined" && window.localStorage !== undefined;
  }

  get(key: string): string | null {
    if (this.isClient()) {
      const value = localStorage.getItem(key);
      console.log(`StorageService.get('${key}')`, { value, exists: !!value });
      return value;
    }
    return null;
  }

  set(key: string, value: string): void {
    if (this.isClient()) {
      console.log(`StorageService.set('${key}')`, { value, length: value?.length });
      localStorage.setItem(key, value);
    }
  }

  remove(key: string): void {
    if (this.isClient()) {
      localStorage.removeItem(key);
      console.log(`StorageService.remove('${key}')`);
    }
  }
}

export default new StorageService();
