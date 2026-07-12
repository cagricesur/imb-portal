import Auth from "@imb-portal/views/Auth";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/auth")({
  component: Auth,
});
