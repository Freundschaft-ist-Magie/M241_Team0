import { useToastStore } from "~/utils/stores/base/ToastStore";

type MessageCallback = (data: any) => void;

interface WebSocketEntry {
  socket: WebSocket | null;
  listeners: Set<MessageCallback>;
  isConnected: boolean;
  retryCount: number;
  isClosingIntentionally: boolean;
  connectTimeoutId: NodeJS.Timeout | null;
}

class SocketService {
  sockets: Record<string, WebSocketEntry> = {};

  /**
   * Constructs the full WebSocket URL (ws:// or wss://)
   * @param endpoint - The relative path (e.g., /api/RoomDatas/ws/1)
   */
  buildUrl(endpoint: string): string {
    // It should NOT contain http:// or ws://
    const host = import.meta.env.VITE_API_URL;
    if (!host) {
      console.error("[WS] VITE_API_URL environment variable is not set!");
      throw new Error("[WS] VITE_API_URL environment variable is not set!");
    }

    if (host.includes("://")) {
      console.warn(`[WS] VITE_API_URL ('${host}') should not include protocol (http://, ws://). Assuming it's just the host/domain.`);
      // Attempt to strip protocol for safety, might not cover all cases
      // host = host.replace(/^.*:\/\//, '');
    }

    // Use wss:// if the main page is served over https://
    const protocol = window.location.protocol === 'https:' ? 'wss://' : 'ws://';
    const prefix = import.meta.env.VITE_API_ENDPOINT_PREFIX || "";

    const cleanPrefix = prefix.endsWith('/') ? prefix.slice(0, -1) : prefix;
    const cleanEndpoint = endpoint.startsWith('/') ? endpoint : `/${endpoint}`;
    const path = cleanPrefix ? `${cleanPrefix}${cleanEndpoint}` : cleanEndpoint;

    const fullUrl = `${protocol}${host}/${path}`;
    console.log(`[WS] Constructed URL: ${fullUrl} for endpoint: ${endpoint}`);
    return fullUrl;
  }

  connect(endpoint: string): void {
    let entry = this.sockets[endpoint];

    // If already connected or connecting, do nothing
    if (entry?.isConnected || (entry?.socket && entry.socket.readyState === WebSocket.CONNECTING)) {
      console.log(`[WS] Already connected or connecting to ${endpoint}. Skipping new connection.`);
      return;
    }

    const fullUrl = this.buildUrl(endpoint);
    if (!fullUrl) return; // Stop if URL couldn't be built

    const toastStore = useToastStore();

    // If entry exists (meaning we are reconnecting), reuse listeners. Otherwise, create new entry.
    if (!entry) {
      entry = {
        socket: null,
        listeners: new Set(),
        isConnected: false,
        retryCount: 0,
        isClosingIntentionally: false,
        connectTimeoutId: null,
      };
      this.sockets[endpoint] = entry;
    } else {
      // Reset flags for reconnection attempt
      entry.isConnected = false;
      entry.isClosingIntentionally = false;
      if (entry.connectTimeoutId) clearTimeout(entry.connectTimeoutId);
      entry.connectTimeoutId = null;
    }

    console.log(`[WS] Attempting to connect to ${endpoint} (URL: ${fullUrl})`);
    try {
      entry.socket = new WebSocket(fullUrl);
    } catch (error) {
      console.error(`[WS] Failed to create WebSocket for ${endpoint}:`, error);
      toastStore.setToast("error", "WebSocket Fehler", `Konnte keine Verbindung zu ${endpoint} herstellen.`);
      // Clean up entry if creation failed immediately
      delete this.sockets[endpoint];
      return;
    }

    entry.socket.onopen = () => {
      if (!this.sockets[endpoint]) return; // Check if entry was removed before connection opened
      console.info(`[WS] Connected to ${endpoint}`);
      this.sockets[endpoint].isConnected = true;
      this.sockets[endpoint].retryCount = 0;
      this.sockets[endpoint].isClosingIntentionally = false; // Ensure flag is reset
    };

    entry.socket.onmessage = (event) => {
      if (!this.sockets[endpoint]) return;
      try {
        const data = JSON.parse(event.data);
        // Use a copy of the listeners set in case a callback modifies the original set during iteration
        new Set(this.sockets[endpoint].listeners).forEach((cb) => cb(data));

        console.log(`[WS] Message received from ${endpoint}:`, data);
      } catch (e) {
        console.error(`[WS] Failed to parse message from ${endpoint}:`, event.data, e);
      }
    };

    entry.socket.onerror = (error) => {
      if (!this.sockets[endpoint]) return;
      console.error(`[WS] Error for ${endpoint}`, error);
    };

    entry.socket.onclose = (event) => {
      const currentEntry = this.sockets[endpoint];
      if (!currentEntry) {
        console.log(`[WS] Connection closed for ${endpoint}, but entry no longer exists. No action taken.`);
        return;
      }

      console.warn(`[WS] Connection closed for ${endpoint}. Code: ${event.code}, Reason: ${event.reason}, Intentional: ${currentEntry.isClosingIntentionally}`);
      currentEntry.isConnected = false;
      currentEntry.socket = null; // Clear the closed socket object

      // --- Reconnection Logic ---
      // Only reconnect if the closure was NOT intentional
      if (!currentEntry.isClosingIntentionally) {
        if (currentEntry.retryCount < 3) { // Max retries
          const delay = 2000 * Math.pow(2, currentEntry.retryCount); // Exponential backoff
          console.info(`[WS] Reconnecting to ${endpoint} in ${delay / 1000}s (attempt ${currentEntry.retryCount + 1})...`);

          // Clear previous timeout just in case
          if (currentEntry.connectTimeoutId) clearTimeout(currentEntry.connectTimeoutId);

          currentEntry.connectTimeoutId = setTimeout(() => {
            if (this.sockets[endpoint]) { // Check if still relevant before reconnecting
              this.sockets[endpoint].retryCount++;
              this.connect(endpoint);
            } else {
              console.log(`[WS] Reconnect aborted for ${endpoint}, entry removed.`);
            }
          }, delay);
        } else {
          console.error(`[WS] Failed to reconnect to ${endpoint} after ${currentEntry.retryCount} attempts. Giving up.`);
          toastStore.setToast("error", "Verbindung verloren", `WebSocket zu ${endpoint} konnte nicht wiederhergestellt werden.`);
          // Clean up the entry if we give up entirely
          delete this.sockets[endpoint];
        }
      } else {
        console.info(`[WS] Connection to ${endpoint} closed intentionally. No reconnection attempt.`);
        // Entry should have been deleted by the 'close' method already.
        // If not, uncomment the next line for safety, but it might hide bugs in 'close'.
        // delete this.sockets[endpoint];
      }
    };
  }

  subscribe(endpoint: string, callback: MessageCallback): void {
    if (endpoint.includes("://")) {
      console.warn(`[WS] subscribe called with full URL '${endpoint}'. Using it directly as key, but expected relative path.`);
    }

    if (!this.sockets[endpoint]) {
      this.connect(endpoint);
    } else if (!this.sockets[endpoint].isConnected && !this.sockets[endpoint].socket) {
      // If entry exists but socket is null (e.g., after failed connection/closure), try connecting again.
      console.log(`[WS] Entry for ${endpoint} exists but not connected. Re-initiating connection.`);
      this.connect(endpoint);
    }

    this.sockets[endpoint]?.listeners.add(callback);
    console.log(`[WS] Listener added for ${endpoint}. Total listeners: ${this.sockets[endpoint]?.listeners.size}`);
  }

  unsubscribe(endpoint: string, callback: MessageCallback): void {
    const entry = this.sockets[endpoint];
    if (entry) {
      entry.listeners.delete(callback);
      console.log(`[WS] Listener removed for ${endpoint}. Remaining listeners: ${entry.listeners.size}`);

      // If no listeners are left, close the connection intentionally
      if (entry.listeners.size === 0) {
        console.log(`[WS] No listeners left for ${endpoint}. Closing connection.`);
        this.close(endpoint);
      }
    } else {
      console.warn(`[WS] Attempted to unsubscribe from non-existent endpoint: ${endpoint}`);
    }
  }

  /**
   * Intentionally closes the WebSocket connection and cleans up.
   */
  close(endpoint: string): void {
    const entry = this.sockets[endpoint];
    if (entry) {
      console.info(`[WS] Intentionally closing connection to ${endpoint}`);
      entry.isClosingIntentionally = true;

      // Clear any pending reconnect timeout
      if (entry.connectTimeoutId) {
        clearTimeout(entry.connectTimeoutId);
        entry.connectTimeoutId = null;
      }

      if (entry.socket) {
        entry.listeners.clear();
        entry.socket.close();
        entry.socket = null;
      } else {
        console.log(`[WS] Socket for ${endpoint} already null during close operation.`);
      }

      // Remove the entry from the main sockets object
      delete this.sockets[endpoint];
    } else {
      console.warn(`[WS] Attempted to close non-existent endpoint: ${endpoint}`);
    }
  }

  /**
   * Closes all managed WebSocket connections.
   */
  closeAll(): void {
    console.log("[WS] Closing all WebSocket connections...");
    Object.keys(this.sockets).forEach(endpoint => {
      this.close(endpoint);
    });
  }
}

export default SocketService;
