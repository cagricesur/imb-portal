import { Image, useComputedColorScheme } from "@mantine/core";

export const PortalLogo: React.FunctionComponent = () => {
  const colorScheme = useComputedColorScheme();

  return (
    <Image
      src={
        colorScheme === "light"
          ? "/logo-black-96x96.png"
          : "/logo-white-96x96.png"
      }
      h={96}
      w={96}
      alt="IMB Portal"
    />
  );
};
