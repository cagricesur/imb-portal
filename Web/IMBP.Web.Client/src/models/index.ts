import type { AuthenticationResponse } from "@imb-portal/api";

export * from "./constants";

export interface IAuthStoreState {
  data?: AuthenticationResponse;
  authenticated: boolean;
  signin: (data: AuthenticationResponse) => void;
  signout: () => void;
}

export interface IAppState {
  authState: IAuthStoreState;
}
