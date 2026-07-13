import Axios, { type AxiosRequestConfig, type InternalAxiosRequestConfig } from "axios";

const ACCESS_TOKEN_REFRESH_INTERVAL_MS = 12 * 60 * 1000;

export const httpClient = Axios.create({
  baseURL: "",
  withCredentials: true,
  headers: { "Content-Type": "application/json" },
});

const refreshClient = Axios.create({
  baseURL: "",
  withCredentials: true,
  headers: { "Content-Type": "application/json" },
});

type RetryableAxiosRequestConfig = InternalAxiosRequestConfig & {
  _retry?: boolean;
};

let refreshPromise: Promise<void> | null = null;
let silentRefreshTimer: ReturnType<typeof setInterval> | null = null;

const shouldSkipRefresh = (url?: string) => {
  if (!url) {
    return true;
  }

  return (
    url.includes("/api/User/refresh") ||
    url.includes("/api/User/authenticate") ||
    url.includes("/api/User/logout")
  );
};

const clearAuthSession = () => {
  void import("@imb-portal/stores").then(({ useAuthStore }) => {
    useAuthStore.getState().clearSession();
  });
};

const redirectToAuth = () => {
  if (window.location.pathname !== "/auth") {
    window.location.assign("/auth");
  }
};

const refreshSession = async () => {
  if (!refreshPromise) {
    refreshPromise = refreshClient
      .post("/api/User/refresh")
      .then(() => undefined)
      .finally(() => {
        refreshPromise = null;
      });
  }

  await refreshPromise;
};

httpClient.interceptors.response.use(
  function (response) {
    return response;
  },
  async function (error) {
    const originalRequest = error.config as RetryableAxiosRequestConfig | undefined;

    if (
      error.response?.status === 401 &&
      originalRequest &&
      !originalRequest._retry &&
      !shouldSkipRefresh(originalRequest.url)
    ) {
      originalRequest._retry = true;

      try {
        await refreshSession();
        return httpClient(originalRequest);
      } catch {
        clearAuthSession();
        redirectToAuth();
        return Promise.reject(error);
      }
    }

    return Promise.reject(error);
  },
);

export const startSilentRefresh = () => {
  stopSilentRefresh();

  silentRefreshTimer = setInterval(() => {
    void import("@imb-portal/stores").then(({ useAuthStore }) => {
      if (!useAuthStore.getState().authenticated) {
        return;
      }

      void refreshSession().catch(() => {
        clearAuthSession();
        redirectToAuth();
      });
    });
  }, ACCESS_TOKEN_REFRESH_INTERVAL_MS);
};

export const stopSilentRefresh = () => {
  if (silentRefreshTimer) {
    clearInterval(silentRefreshTimer);
    silentRefreshTimer = null;
  }
};

export const HttpService = async <T>(
  config: AxiosRequestConfig,
  options?: AxiosRequestConfig,
): Promise<T> => {
  const response = await httpClient({ ...config, ...options });
  return response?.data;
};
