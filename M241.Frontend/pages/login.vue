<script lang="ts" setup>
import { useAuthStore } from "@/utils/stores/base/AuthStore";
import { useLoadingStore } from "@/utils/stores/base/LoadingStore";
import { useToastStore } from "@/utils/stores/base/ToastStore";
import { get, post } from "@/utils/services/base/ApiService";

definePageMeta({
  layout: "not-logged-in",
});

// Form data
const email = ref("");
const password = ref("");
const loading = ref(false);
const router = useRouter();

// Field-specific errors
const emailError = ref("");
const passwordError = ref("");

const loadingStore = useLoadingStore();
const toastStore = useToastStore();
const authStore = useAuthStore();

const validateEmail = (): boolean => {
  emailError.value = ""; // Reset error
  if (!email.value || email.value.trim() === "") {
    emailError.value = "E-Mail ist erforderlich.";
    return false;
  }
  // Basic email format check using regex
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(email.value)) {
    emailError.value = "Ungültiges E-Mail-Format.";
    return false;
  }
  return true;
};

const validatePassword = (): boolean => {
  passwordError.value = "";
  if (!password.value) {
    passwordError.value = "Passwort ist erforderlich.";
    return false;
  }

  return true;
};

const isFormValid = computed(() => {
  return !emailError.value && !passwordError.value;
});

const handleLogin = async () => {
  const isEmailValid = validateEmail();
  const isPasswordValid = validatePassword();

  if (!isFormValid.value) {
    console.log("Form is invalid. Errors:", {
      email: emailError.value,
      password: passwordError.value,
    });
    return;
  }

  loading.value = true;
  try {
    const auth = await post("login?useCookies=false&useSessionCookies=false", {
      email: email.value,
      password: password.value,
    });

    authStore.setTokens(auth.accessToken, auth.refreshToken);
    //const _role = await get("/roles"); // You might fetch user details here
    authStore.setRole("Administrator");

    toastStore.setToast("success", "Erfolg", "Erfolgreich eingeloggt!");
    router.push({ path: "/" });
  } catch (err: any) {
    console.error("Login failed:", err);

    if (err.response && err.response.status === 401) {
      toastStore.setToast("error", "Fehler", "Falsche E-Mail oder Passwort");
    } else {
      toastStore.setToast(
        "error",
        "Fehler",
        "Ein unerwarteter Fehler ist aufgetreten. Bitte versuchen Sie es erneut."
      );
    }
  } finally {
    loading.value = false;
  }
};

loadingStore.setLoading(true);

onMounted(() => {
  loadingStore.setLoading(false);
});
</script>

<template>
  <div class="bg-gray-100 p-4 flex items-center justify-center min-h-screen">
    <div class="mt-4 p-6 bg-white shadow-lg rounded-lg w-full max-w-md">
      <div class="flex justify-center mb-4">
        <i class="pi pi-user text-primary1" style="font-size: 3rem"></i>
      </div>

      <h2 class="text-center text-2xl text-gray-800 font-semibold mb-6">Login</h2>

      <form @submit.prevent="handleLogin" class="flex flex-col gap-4">
        <div>
          <label for="email_label" class="block text-sm font-medium text-gray-700 mb-1"
            >E-Mail</label
          >
          <InputText
            type="email"
            id="email_label"
            class="w-full"
            inputClass="w-full"
            v-model="email"
            @blur="validateEmail"
            :invalid="!!emailError"
            placeholder="max.mustermann@muster.com"
            aria-describedby="email-error"
          />

          <small
            v-if="emailError"
            id="email-error"
            class="p-error text-xs text-red-600 mt-1 block"
          >
            {{ emailError }}
          </small>
        </div>

        <div>
          <label for="password_label" class="block text-sm font-medium text-gray-700 mb-1"
            >Passwort</label
          >
          <Password
            id="password_label"
            v-model="password"
            @blur="validatePassword"
            toggleMask
            :feedback="false"
            placeholder="Passwort eingeben"
            class="w-full"
            inputClass="w-full"
            :invalid="!!passwordError"
            aria-describedby="password-error"
          />

          <small
            v-if="passwordError"
            id="password-error"
            class="p-error text-xs text-red-600 mt-1 block"
          >
            {{ passwordError }}
          </small>
        </div>

        <div class="flex justify-end">
          <NuxtLink to="/forgot-password" class="text-sm text-primary1 hover:underline">
            Passwort vergessen?
          </NuxtLink>
        </div>

        <Button
          icon="pi pi-sign-in"
          label="Einloggen"
          type="submit"
          severity="info"
          class="w-full text-white! bg-primary1! hover:bg-primary1/90! active:bg-primary2! py-2.5 mt-2"
          :loading="loading"
          :disabled="loading"
        />
      </form>
    </div>
  </div>
</template>
