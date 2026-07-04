import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/login")({
  component: RouteComponent,
});

function RouteComponent() {
  const nav = Route.useNavigate();
  return (
    <div>
      <span>Hello "/auth"!</span>
      <button
        onClick={() => {
          nav({ to: "/" });
        }}
      >
        Login
      </button>
    </div>
  );
}
