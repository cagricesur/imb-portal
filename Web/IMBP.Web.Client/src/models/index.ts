import type { AuthenticationResponse } from "@imb-portal/api";

export * from "./constants";

export interface IAuthStoreState {
  data?: AuthenticationResponse;
  authenticated: boolean;
  bootstrapped: boolean;
  signin: (data: AuthenticationResponse) => void;
  signout: () => void;
  setBootstrapped: (bootstrapped: boolean) => void;
}

export interface IAppState {
  authState: IAuthStoreState;
}
