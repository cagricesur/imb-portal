import { createTheme, type MantineColorsTuple } from "@mantine/core";

const ZTRED: MantineColorsTuple = [
  "#ffe8ea",
  "#ffd0d3",
  "#fd9ea4",
  "#fb6972",
  "#f93d48",
  "#f8222d",
  "#f9131f",
  "#e20514",
  "#c60010",
  "#ae000a",
];

export const theme = createTheme({
  colors: { ZTRED },
});
