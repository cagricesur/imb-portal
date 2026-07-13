import { getUser } from "@imb-portal/api";
import type { IAuthStoreState, UserProfile } from "@imb-portal/models";
import { create } from "zustand";

const isAuthenticatedProfile = (data?: UserProfile) => {
  return Boolean(data?.userName);
};

export const useAuthStore = create<IAuthStoreState>((set, get) => ({
  authenticated: false,
  initialized: false,
  data: undefined,
  signin(data) {
    set({
      data,
      authenticated: isAuthenticatedProfile(data),
    });
  },
  clearSession() {
    set({
      data: undefined,
      authenticated: false,
    });
  },
  async signout() {
    try {
      await getUser().postApiUserLogout();
    } catch {
      // Session may already be invalid; clear local state regardless.
    }

    get().clearSession();
  },
  async initialize() {
    if (get().initialized) {
      return;
    }

    try {
      const profile = await getUser().getApiUserMe();
      if (isAuthenticatedProfile(profile)) {
        set({
          data: profile,
          authenticated: true,
          initialized: true,
        });
        return;
      }
    } catch {
      get().clearSession();
    }

    set({ initialized: true });
  },
}));
