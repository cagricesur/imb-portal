import type { IAuthStoreState } from "@imb-portal/models";
import { create } from "zustand";

export const useAuthStore = create<IAuthStoreState>((set) => ({
  authenticated: false,
  signin(data) {
    const { token, ...rest } = data;
    const authenticated = token ? true : false;
    set({
      data: authenticated ? { token, ...rest } : undefined,
      authenticated,
    });
  },
  signout() {
    set({ data: undefined, authenticated: false });
  },
}));
