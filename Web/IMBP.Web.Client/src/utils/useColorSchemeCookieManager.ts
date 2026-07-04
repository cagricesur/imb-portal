import { PortalContants } from "@imb-portal/models";
import {
  isMantineColorScheme,
  type MantineColorScheme,
  type MantineColorSchemeManager,
} from "@mantine/core";
import { useCookies } from "react-cookie";

const COOKIE_KEY = PortalContants.CookieKeys.ColorScheme;

export function useColorSchemeCookieManager(): MantineColorSchemeManager {
  const [cookies, setCookie, removeCookie] = useCookies([COOKIE_KEY]);

  return {
    get(defaultValue: MantineColorScheme) {
      const stored = cookies[COOKIE_KEY];
      return isMantineColorScheme(stored) ? stored : defaultValue;
    },
    set(value: MantineColorScheme) {
      setCookie(COOKIE_KEY, value, { path: "/" });
    },
    clear() {
      removeCookie(COOKIE_KEY);
    },
    subscribe() {},
    unsubscribe() {},
  } as MantineColorSchemeManager;
}
