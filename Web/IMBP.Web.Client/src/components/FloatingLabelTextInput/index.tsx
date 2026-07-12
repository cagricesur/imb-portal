import { TextInput, type TextInputProps } from "@mantine/core";
import classes from "./index.module.scss";

export const FloatingLabelTextInput: React.FunctionComponent<TextInputProps> = (
  props,
) => {
  return (
    <TextInput
      {...props}
      classNames={{
        ...classes,
        root: props.label ? classes["root"] : classes["root-no-label"],
      }}
    />
  );
};
