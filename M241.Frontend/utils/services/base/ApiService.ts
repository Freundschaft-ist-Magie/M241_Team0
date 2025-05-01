import { useAuthStore } from "~/utils/stores/base/AuthStore";
import { useToastStore } from "~/utils/stores/base/ToastStore";
import StorageService from "./StorageService";

const API_BASE_URL = "http://" + import.meta.env.VITE_API_URL + "/";
const VITE_API_ENDPOINT_PREFIX = import.meta.env.VITE_API_ENDPOINT_PREFIX || "";

const _makeApiCall = async (
  method: "GET" | "POST" | "PUT" | "DELETE",
  endpoint: string,
  data: object | null = null,
  params: object | null = null,
  retryCount: number = 0
): Promise<any> => {
  const authStore = useAuthStore();
  const toastStore = useToastStore();

  try {
    const response = await $fetch(`${API_BASE_URL}${VITE_API_ENDPOINT_PREFIX}${endpoint}`, {
      method,
      body: data ? JSON.stringify(data) : undefined,
      query: params || undefined,
      headers: {
        "Content-Type": "application/json",
        "Accept": "application/json",
        ...(authStore.accessToken ? { Authorization: `Bearer ${authStore.accessToken}` } : {}),
      },
      credentials: "include",
    });

    console.info("API call successful:", response);
    return response;
  } catch (error: any) {
    if (error.response?.status === 401 && authStore.refreshToken && retryCount < 1) {
      console.warn("Access token expired, attempting refresh...");

      try {
        const refreshResponse = await $fetch(`${API_BASE_URL}/refresh`, {
          method: "POST",
          body: { refreshToken: authStore.refreshToken },
        });

        const { accessToken } = refreshResponse;
        authStore.accessToken = accessToken;
        StorageService.set("accessToken", accessToken);

        return _makeApiCall(method, endpoint, data, params, retryCount + 1);
      } catch (refreshError) {
        console.error("Token refresh failed:", refreshError);
        authStore.clearUser();
        throw refreshError;
      }
    }

    console.error("Error in API call:", error);
    toastStore.setToast(
      "error",
      "Fehler",
      "Fehler beim Verbinden zum Server."
    );
    throw error;
  }
};

/**
 * Makes a GET request to the specified endpoint.
 *
 * @param {string} endpoint - The API endpoint to call.
 * @param {object | null} [params=null] - Query parameters to include in the request.
 * @returns {Promise<any>} The response data from the API.
 */
export async function get(endpoint: string, params: object | null = null): Promise<any> {
  return _makeApiCall("GET", endpoint, null, params);
}

/**
 * Makes a POST request to the specified endpoint.
 *
 * @param {string} endpoint - The API endpoint to call.
 * @param {object | null} [data=null] - The body data to include in the request.
 * @param {object | null} [params=null] - Query parameters to include in the request.
 * @returns {Promise<any>} - The response data from the API.
 */
export async function post(endpoint: string, data: object | null = null, params: object | null = null): Promise<any> {
  return _makeApiCall("POST", endpoint, data, params);
}

/**
 * Makes a PUT request to the specified endpoint.
 *
 * @param {string} endpoint - The API endpoint to call.
 * @param {object | null} [data=null] - The body data to include in the request.
 * @param {object | null} [params=null] - Query parameters to include in the request.
 * @returns {Promise<any>} - The response data from the API.
 */
export async function put(endpoint: string, data: object | null = null, params: object | null = null): Promise<any> {
  return _makeApiCall("PUT", endpoint, data, params);
}

/**
 * Makes a DELETE request to the specified endpoint.
 *
 * @param {string} endpoint - The API endpoint to call.
 * @param {object | null} [data=null] - The body data to include in the request.
 * @param {object | null} [params=null] - Query parameters to include in the request.
 * @returns {Promise<any>} - The response data from the API.
 */
export async function del(endpoint: string, data: object | null = null, params: object | null = null): Promise<any> {
  return _makeApiCall("DELETE", endpoint, data, params);
}
