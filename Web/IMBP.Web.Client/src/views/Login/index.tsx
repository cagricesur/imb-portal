import {
  Button,
  Paper,
  PasswordInput,
  Stack,
  TextInput,
  Title,
} from "@mantine/core";
import { useForm } from "@mantine/form";

import classes from "./index.module.scss";

type LoginFormValues = {
  username: string;
  password: string;
};

const Login: React.FunctionComponent = () => {
  const form = useForm<LoginFormValues>({
    initialValues: {
      username: "",
      password: "",
    },
    validate: {
      username: (value) =>
        value.trim().length > 0 ? null : "Username is required",
      password: (value) =>
        value.length > 0 ? null : "Password is required",
    },
  });

  const handleSubmit = form.onSubmit(() => {
    // TODO: wire up authentication
  });

  return (
    <div className={classes.page}>
      <div aria-hidden className={classes.overlay} />
      <Paper
        className={classes.form}
        component="section"
        p={{ base: "md", sm: "xl" }}
        radius="lg"
        shadow="xl"
        withBorder
      >
        <form onSubmit={handleSubmit}>
          <Stack gap="md">
            <Title order={2} ta="center">
              Sign in
            </Title>

            <TextInput
              autoComplete="username"
              label="Username"
              placeholder="Enter your username"
              required
              size="md"
              {...form.getInputProps("username")}
            />

            <PasswordInput
              autoComplete="current-password"
              label="Password"
              placeholder="Enter your password"
              required
              size="md"
              {...form.getInputProps("password")}
            />

            <Button fullWidth mt="xs" size="md" type="submit">
              Login
            </Button>
          </Stack>
        </form>
      </Paper>
    </div>
  );
};

export default Login;
