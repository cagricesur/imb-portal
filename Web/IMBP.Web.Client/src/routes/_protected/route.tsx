import { createFileRoute } from "@tanstack/react-router";
import Master from "../-master";

export const Route = createFileRoute("/_protected")({
  component: Master,
});
