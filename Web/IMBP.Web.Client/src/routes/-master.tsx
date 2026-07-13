import { Button, Group, Text } from "@mantine/core";
import { useAuthStore } from "@imb-portal/stores";
import { useRouter } from "@tanstack/react-router";
import { Outlet } from "@tanstack/react-router";

const Master: React.FunctionComponent = () => {
  const signout = useAuthStore((state) => state.signout);
  const data = useAuthStore((state) => state.data);
  const router = useRouter();

  const handleLogout = async () => {
    await signout();
    await router.navigate({ to: "/auth" });
  };

  return (
    <div>
      <Group justify="space-between" p="md">
        <Text>{data?.fullName ?? data?.userName}</Text>
        <Button variant="light" onClick={() => void handleLogout()}>
          Logout
        </Button>
      </Group>
      <div>
        <Outlet />
      </div>
    </div>
  );
};

export default Master;
