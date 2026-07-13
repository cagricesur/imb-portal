import type { AuthenticationResponse } from "@imb-portal/api";

export * from "./constants";

export type UserProfile = Omit<AuthenticationResponse, "token">;

export interface IAuthStoreState {
  data?: UserProfile;
  authenticated: boolean;
  initialized: boolean;
  signin: (data: UserProfile) => void;
  clearSession: () => void;
  signout: () => Promise<void>;
  initialize: () => Promise<void>;
}

export interface IAppState {
  authState: IAuthStoreState;
}
