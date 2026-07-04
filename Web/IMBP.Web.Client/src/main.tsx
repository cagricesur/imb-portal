import App from "@imb-portal/App";
import { StrictMode } from "react";
import { CookiesProvider } from "react-cookie";
import ReactDOM from "react-dom/client";

const rootElement = document.getElementById("root")!;
if (!rootElement.innerHTML) {
  const root = ReactDOM.createRoot(rootElement);
  root.render(
    <StrictMode>
      <CookiesProvider>
        <App />
      </CookiesProvider>
    </StrictMode>,
  );
}
