import Home from "@imb-portal/views/Home";
import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/_protected/")({
  component: Home,
  beforeLoad(ctx) {
    if (!ctx.context.authState.initialized) {
      return;
    }

    if (!ctx.context.authState.authenticated) {
      throw redirect({ to: "/auth" });
    }
  },
});
