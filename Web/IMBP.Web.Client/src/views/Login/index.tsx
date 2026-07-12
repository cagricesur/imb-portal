import {
  FloatingLabelPasswordInput,
  FloatingLabelTextInput,
} from "@imb-portal/components";
import {
  Button,
  Divider,
  Flex,
  Group,
  Image,
  Paper,
  Stack,
  Switch,
  Title,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useState } from "react";
import classnames from "./index.module.scss";

type LoginFormValues = {
  username: string;
  password: string;
  remember: boolean;
};

const Login: React.FunctionComponent = () => {
  const [loading, setLoading] = useState<boolean>(false);
  const form = useForm<LoginFormValues>({
    initialValues: {
      username: "",
      password: "",
      remember: false,
    },
    validate: {
      username: (value) =>
        value.trim().length > 0 ? null : "Username is required",
      password: (value) => (value.length > 0 ? null : "Password is required"),
    },
  });

  const handleSubmit = form.onSubmit(() => {
    // TODO: wire up authentication
  });

  return (
    <Flex className={classnames.screen}>
      <Paper radius="lg" shadow="xl" className={classnames.root}>
        <Stack align="stretch" gap={0}>
          <Group justify="center">
            {/* <Image src={logo} h={64} w={64} alt="NoobzCord" /> */}
          </Group>
          <Title>IMBP</Title>

          <Divider my={32} />

          <form onSubmit={handleSubmit}>
            <Stack>
              <FloatingLabelTextInput
                label="Kullancı Adı"
                placeholder="Kullancı Adı"
                value={form.values.username}
                onChange={(event) =>
                  form.setFieldValue("username", event.currentTarget.value)
                }
                error={form.errors.username}
                disabled={loading}
                radius="md"
              />

              <FloatingLabelPasswordInput
                label="Şifre"
                placeholder="Şifre"
                value={form.values.password}
                onChange={(event) =>
                  form.setFieldValue("password", event.currentTarget.value)
                }
                error={form.errors.password}
                disabled={loading}
                radius="md"
              />

              <Switch
                label="Beni Hatırla"
                checked={form.values.remember}
                disabled={loading}
                onChange={(event) => {
                  form.setFieldValue("remember", event.currentTarget.checked);
                }}
              />

              <Button type="submit" radius="md" loading={loading}>
                Giriş Yap
              </Button>
            </Stack>
          </form>
        </Stack>
      </Paper>
    </Flex>
  );
};

export default Login;
