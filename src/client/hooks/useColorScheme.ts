import {
  ColorSchemeName,
  useColorScheme as useReactNativeColorScheme,
} from "react-native";

export function useColorScheme(): Extract<ColorSchemeName, "light" | "dark"> {
  return useReactNativeColorScheme() ?? "light";
}
