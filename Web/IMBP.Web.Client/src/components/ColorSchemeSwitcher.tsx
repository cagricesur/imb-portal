import {
  ActionIcon,
  Tooltip,
  useComputedColorScheme,
  useMantineColorScheme,
} from "@mantine/core";
import { IconMoon, IconSun } from "@tabler/icons-react";
import { useTranslation } from "react-i18next";

export const ColorSchemeSwitcher: React.FunctionComponent = () => {
  const { t } = useTranslation();
  const { toggleColorScheme } = useMantineColorScheme();
  const colorScheme = useComputedColorScheme();

  return (
    <Tooltip
      label={
        colorScheme === "dark"
          ? t("COMPONENTS.COLORSCHEMESWITCHER.TOOLTIP.LIGHT")
          : t("COMPONENTS.COLORSCHEMESWITCHER.TOOLTIP.DARK")
      }
      position="bottom"
    >
      <ActionIcon
        variant="filled"
        size="md"

        onClick={() => toggleColorScheme()}
      >
        {colorScheme === "dark" ? (
          <IconSun size={20} />
        ) : (
          <IconMoon size={20} />
        )}
      </ActionIcon>
    </Tooltip>
  );
};
