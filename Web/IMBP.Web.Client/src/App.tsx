import { routeTree } from "@imb-portal/routeTree.gen";
import { startSilentRefresh, stopSilentRefresh } from "@imb-portal/api/httpService";
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
  const colorScheme = useColorScheme();
  const colorSchemeCookieManager = useColorSchemeCookieManager();

  useEffect(() => {
    void useAuthStore.getState().initialize();
  }, []);

  useEffect(() => {
    if (!authState.initialized || !authState.authenticated) {
      stopSilentRefresh();
      return;
    }

    startSilentRefresh();
    return () => {
      stopSilentRefresh();
    };
  }, [authState.authenticated, authState.initialized]);

  if (!authState.initialized) {
    return (
      <MantineProvider
        theme={theme}
        defaultColorScheme={cs}
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
