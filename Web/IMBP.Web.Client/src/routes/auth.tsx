import Auth from "@imb-portal/views/Auth";
import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/auth")({
  component: Auth,
  beforeLoad(ctx) {
    if (ctx.context.authState.authenticated) {
      throw redirect({ to: "/" });
    }
  },
});
