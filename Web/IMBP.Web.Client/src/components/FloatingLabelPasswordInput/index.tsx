import { PasswordInput, type PasswordInputProps } from "@mantine/core";
import classes from "./index.module.scss";

export const FloatingLabelPasswordInput: React.FunctionComponent<
  PasswordInputProps
> = (props) => {
  return (
    <PasswordInput
      {...props}
      classNames={{
        ...classes,
        root: props.label ? classes["root"] : classes["root-no-label"],
      }}
    />
  );
};
