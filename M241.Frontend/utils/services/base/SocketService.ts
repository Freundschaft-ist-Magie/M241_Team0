import { useToastStore } from "~/utils/stores/base/ToastStore";

type MessageCallback = (data: any) => void;

interface WebSocketEntry {
  socket: WebSocket;
  listeners: Set<MessageCallback>;
  isConnected: boolean;
  retryCount: number;
}

class SocketService {
  BASE_WS_URL = `ws://${import.meta.env.VITE_API_URL}/${import.meta.env.VITE_API_ENDPOINT_PREFIX || ""}`;
  sockets: Record<string, WebSocketEntry> = {};

  buildUrl(endpoint: string): string {
    return `${this.BASE_WS_URL}${endpoint}`;
  }

  connect(endpoint: string): void {
    const fullUrl = this.buildUrl(endpoint);

    if (this.sockets[endpoint]?.isConnected) {
      console.warn(`WebSocket already connected for ${endpoint}`);
      return;
    }

    const toastStore = useToastStore();
    const entry: WebSocketEntry = {
      socket: new WebSocket(fullUrl),
      listeners: new Set(),
      isConnected: false,
      retryCount: 0,
    };

    this.sockets[endpoint] = entry;

    entry.socket.onopen = () => {
      console.info(`[WS] Connected to ${endpoint}`);
      entry.isConnected = true;
      entry.retryCount = 0;
    };

    entry.socket.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        entry.listeners.forEach((cb) => cb(data));
      } catch (e) {
        console.error(`[WS] Failed to parse message from ${endpoint}`, e);
      }
    };

    entry.socket.onerror = (error) => {
      console.error(`[WS] Error for ${endpoint}`, error);
      toastStore.setToast("error", "WebSocket Fehler", `Verbindung zu ${endpoint} fehlgeschlagen.`);
    };

    entry.socket.onclose = () => {
      console.warn(`[WS] Connection closed for ${endpoint}`);
      entry.isConnected = false;

      if (entry.retryCount < 3) {
        setTimeout(() => {
          console.info(`[WS] Reconnecting to ${endpoint}...`);
          entry.retryCount++;
          this.connect(endpoint);
        }, 2000);
      } else {
        toastStore.setToast("error", "Verbindung verloren", `WebSocket zu ${endpoint} konnte nicht wiederhergestellt werden.`);
      }
    };
  }

  subscribe(endpoint: string, callback: MessageCallback): void {
    if (!this.sockets[endpoint]) {
      this.connect(endpoint);
    }

    this.sockets[endpoint]?.listeners.add(callback);
  }

  unsubscribe(endpoint: string, callback: MessageCallback): void {
    this.sockets[endpoint]?.listeners.delete(callback);

    // Clean up socket if no listeners left
    if (this.sockets[endpoint]?.listeners.size === 0) {
      this.close(endpoint);
    }
  }

  close(endpoint: string): void {
    const entry = this.sockets[endpoint];
    if (entry) {
      entry.socket.close();
      delete this.sockets[endpoint];
      console.info(`[WS] Closed connection to ${endpoint}`);
    }
  }
}

export default SocketService;
