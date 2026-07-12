import { Image } from "@mantine/core";
import { useColorScheme } from "@mantine/hooks";

export const PortalLogo: React.FunctionComponent = () => {
  const colorScheme = useColorScheme();
  return (
    <Image
      src={
        colorScheme === "dark"
          ? "/logo-white-96x96.png"
          : "/logo-black-96x96.png"
      }
      h={64}
      w={64}
      alt="IMB-Portal"
    />
  );
};
