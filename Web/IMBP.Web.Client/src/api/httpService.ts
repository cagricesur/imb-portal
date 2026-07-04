import Axios, { type AxiosRequestConfig } from "axios";

const httpService = Axios.create({
  baseURL: "",
  headers: { "Content-Type": "application/json" },
});

// Add a request interceptor
httpService.interceptors.request.use(
  function (config) {
    return config;
  },
  function (error) {
    return Promise.reject(error);
  },
);

// Add a response interceptor
httpService.interceptors.response.use(
  function (response) {
    // Any status code that lie within the range of 2xx cause this function to trigger
    // Do something with response data
    return response;
  },
  function (error) {
    // Any status codes that falls outside the range of 2xx cause this function to trigger
    // Do something with response error
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
