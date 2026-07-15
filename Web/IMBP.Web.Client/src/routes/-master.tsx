import { getUser } from "@imb-portal/api";
import { useAuthStore } from "@imb-portal/stores";
import { Button, Group } from "@mantine/core";
import { useRouter, Outlet } from "@tanstack/react-router";
import { useState } from "react";

const Master: React.FunctionComponent = () => {
  const router = useRouter();
  const signout = useAuthStore((state) => state.signout);
  const userName = useAuthStore((state) => state.data?.userName);
  const [loggingOut, setLoggingOut] = useState(false);

  const handleLogout = () => {
    setLoggingOut(true);
    getUser()
      .postApiUserLogout()
      .catch(() => undefined)
      .finally(() => {
        signout();
        setLoggingOut(false);
        void router.navigate({ to: "/auth" });
      });
  };

  return (
    <div>
      <Group justify="space-between" p="md">
        <span>Master</span>
        <Group>
          {userName ? <span>{userName}</span> : null}
          <Button variant="light" loading={loggingOut} onClick={handleLogout}>
            Logout
          </Button>
        </Group>
      </Group>
      <div>
        <Outlet />
      </div>
    </div>
  );
};

export default Master;
