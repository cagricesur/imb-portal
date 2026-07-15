import Axios, { type AxiosError, type AxiosRequestConfig } from "axios";

const httpService = Axios.create({
  baseURL: "",
  withCredentials: true,
  headers: { "Content-Type": "application/json" },
});

httpService.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    const status = error.response?.status;
    const url = error.config?.url ?? "";
    const isAuthBootstrap =
      url.includes("/api/User/authenticate") || url.includes("/api/User/me");

    if (status === 401 && !isAuthBootstrap && typeof window !== "undefined") {
      window.dispatchEvent(new CustomEvent("imb-portal:unauthorized"));
    }

    return Promise.reject(error);
  },
);

export const HttpService = async <T>(
  config: AxiosRequestConfig,
  options?: AxiosRequestConfig,
): Promise<T> => {
  const response = await httpService({ ...config, ...options });
  return response?.data;
};
