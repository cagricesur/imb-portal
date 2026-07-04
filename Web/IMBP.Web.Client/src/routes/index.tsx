import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
  component: RouteComponent,
});

function RouteComponent() {
  const nav = Route.useNavigate();
  return (
    <div>
      <span>Hello "/"!</span>
      <button
        onClick={() => {
          nav({ to: "/home" });
        }}
      >
        Login
      </button>
    </div>
  );
}
