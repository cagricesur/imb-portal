import type { IAppState } from "@imb-portal/models";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";

export const Route = createRootRouteWithContext<IAppState>()({
  component: Outlet,
});
