// https://stackoverflow.com/questions/77658157/how-to-integrate-toast-prive-into-nuxt3
// https://github.com/primefaces/primevue-nuxt-module/issues/27
import DialogService from 'primevue/dialogservice';

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.use(DialogService);
});