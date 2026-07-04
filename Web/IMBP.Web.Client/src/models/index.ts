export * from "./constants";

export interface IAuthStoreState {
  authenticated: boolean;
}
export interface IAppState {
  authState: IAuthStoreState;
}
