import { routeTree } from "@imb-portal/routeTree.gen";
import { theme } from "@imb-portal/theme";
import { LoadingOverlay, MantineProvider } from "@mantine/core";
import { createRouter, RouterProvider } from "@tanstack/react-router";

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
  return (
    <MantineProvider theme={theme} defaultColorScheme="dark">
      <RouterProvider
        router={router}
        context={{ authState: { authenticated: false } }}
      />
    </MantineProvider>
  );
};

export default App;
