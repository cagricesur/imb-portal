import type { IAuthStoreState } from "@imb-portal/models";
import { create } from "zustand";

export const useAuthStore = create<IAuthStoreState>((set) => ({
  authenticated: false,
  bootstrapped: false,
  signin(data) {
    set({
      data,
      authenticated: true,
      bootstrapped: true,
    });
  },
  signout() {
    set({ data: undefined, authenticated: false, bootstrapped: true });
  },
  setBootstrapped(bootstrapped) {
    set({ bootstrapped });
  },
}));
