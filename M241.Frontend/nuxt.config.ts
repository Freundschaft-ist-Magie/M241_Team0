import tailwindcss from "@tailwindcss/vite";
import Aura from '@primeuix/themes/aura';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-11-01',
  devtools: { enabled: true },
  ssr: false,
  modules: ['@primevue/nuxt-module', '@nuxt/icon', '@pinia/nuxt'],

  // https://tailwindcss.com/docs/installation/framework-guides/nuxt
  css: ['~/assets/css/main.css'],
  vite: {
    plugins: [
      tailwindcss(),
    ],
  },

  // https://primevue.org/nuxt/
  primevue: {
    options: {
      ripple: true,
      inputVariant: 'filled',
      theme: {
        preset: Aura,
        options: {
          prefix: 'p',
          darkModeSelector: 'light',
          cssLayer: false
        }
      }
    }
  },

  // For Nuxt 3, global middleware should be in the app directory
  // We'll create a plugin to handle the auth logic globally instead of using routeRules
  routeRules: {
    '/login': {
      // Login page specific rules if needed
    }
  },

  app: {
    head: {
      link: [
        {
          rel: 'icon',
          type: 'image/x-icon',
          href: '/favicon.svg'
        }
      ]
    }
  }
})