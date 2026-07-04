import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_protected/")({
  component: RouteComponent,
});

function RouteComponent() {
  const nav = Route.useNavigate();
  return (
    <div>
      <span>Hello "/"!</span>
      <button
        onClick={() => {
          nav({ to: "/login" });
        }}
      >
        Logout
      </button>
    </div>
  );
}
