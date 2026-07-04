import { defineConfig } from "orval";

export default defineConfig({
  imbportal: {
    input: {
      target: "./openapi.json",
    },
    output: {
      workspace: "src/api",
      mode: "tags",
      clean: true,
      target: "./generated/api.ts",
      schemas: "./generated/models",
      client: "axios",
      override: {
        mutator: {
          path: "./httpService.ts",
          name: "HttpService",
        },
      },
    },
    hooks: {
      afterAllFilesWrite: "prettier --write",
    },
  },
});
