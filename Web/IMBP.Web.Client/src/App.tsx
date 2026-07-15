import { getUser } from "@imb-portal/api";
import { routeTree } from "@imb-portal/routeTree.gen";
import { useAuthStore } from "@imb-portal/stores";
import { theme } from "@imb-portal/theme";
import { useColorSchemeCookieManager } from "@imb-portal/utils";
import { LoadingOverlay, MantineProvider } from "@mantine/core";
import { createRouter, RouterProvider } from "@tanstack/react-router";
import { useColorScheme } from "@mantine/hooks";
import { useEffect } from "react";

import "@mantine/carousel/styles.css";
import "@mantine/charts/styles.css";
import "@mantine/code-highlight/styles.css";
import "@mantine/core/styles.css";
import "@mantine/dates/styles.css";
import "@mantine/dropzone/styles.css";
import "@mantine/notifications/styles.css";
import "@mantine/nprogress/styles.css";
import "@mantine/spotlight/styles.css";
import "@mantine/tiptap/styles.css";

import "@imb-portal/theme/theme.scss";

import "@imb-portal/dayjs";
import "@imb-portal/i18n";

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

const PendingComponent: React.FunctionComponent = () => {
  return (
    <LoadingOverlay
      visible
      overlayProps={{ fixed: true, blur: 5 }}
      loaderProps={{ type: "bars" }}
    />
  );
};

const router = createRouter({
  routeTree,
  context: {
    authState: undefined!,
  },
  defaultPendingComponent: PendingComponent,
});

const App: React.FunctionComponent = () => {
  const authState = useAuthStore();
  const signin = useAuthStore((state) => state.signin);
  const signout = useAuthStore((state) => state.signout);
  const bootstrapped = useAuthStore((state) => state.bootstrapped);
  const colorScheme = useColorScheme();
  const colorSchemeCookieManager = useColorSchemeCookieManager();

  useEffect(() => {
    const api = getUser();
    api
      .getApiUserMe()
      .then((response) => {
        if (response?.userName) {
          signin(response);
        } else {
          signout();
        }
      })
      .catch(() => {
        signout();
      });
  }, [signin, signout]);

  useEffect(() => {
    const onUnauthorized = () => {
      signout();
      void router.navigate({ to: "/auth" });
    };

    window.addEventListener("imb-portal:unauthorized", onUnauthorized);
    return () => {
      window.removeEventListener("imb-portal:unauthorized", onUnauthorized);
    };
  }, [signout]);

  if (!bootstrapped) {
    return (
      <MantineProvider
        theme={theme}
        defaultColorScheme={colorScheme}
        colorSchemeManager={colorSchemeCookieManager}
      >
        <PendingComponent />
      </MantineProvider>
    );
  }

  return (
    <MantineProvider
      theme={theme}
      defaultColorScheme={colorScheme}
      colorSchemeManager={colorSchemeCookieManager}
    >
      <RouterProvider router={router} context={{ authState }} />
    </MantineProvider>
  );
};

export default App;
