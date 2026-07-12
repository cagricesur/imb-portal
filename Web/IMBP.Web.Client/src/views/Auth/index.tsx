import {
  ColorSchemeSwitcher,
  FloatingLabelPasswordInput,
  FloatingLabelTextInput,
  LanguageSwitcher,
  PortalLogo,
} from "@imb-portal/components";
import { PortalContants } from "@imb-portal/models";
import {
  Button,
  Divider,
  Flex,
  Group,
  Paper,
  Stack,
  Switch,
  Text,
} from "@mantine/core";
import { isNotEmpty, useForm } from "@mantine/form";
import { useEffect, useState } from "react";
import { useCookies } from "react-cookie";

import { getUser, type ServiceError } from "@imb-portal/api";
import { useAuthStore } from "@imb-portal/stores";
import { useRouter } from "@tanstack/react-router";
import type { AxiosError } from "axios";
import dayjs from "dayjs";
import classnames from "./index.module.scss";

type AuthFormValues = {
  username: string;
  password: string;
  remember: boolean;
};

const Auth: React.FunctionComponent = () => {
  const [loading, setLoading] = useState<boolean>(false);
  const signin = useAuthStore((state) => state.signin);
  const authenticated = useAuthStore((state) => state.authenticated);
  const router = useRouter();

  const [cookies, setCookie, removeCookie] = useCookies([
    PortalContants.CookieKeys.Authentication.UserName,
    PortalContants.CookieKeys.Authentication.RememberMe,
  ]);

  useEffect(() => {
    if (authenticated) {
      router.navigate({ to: "/" });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [authenticated]);

  const form = useForm<AuthFormValues>({
    mode: "uncontrolled",
    validateInputOnChange: true,
    clearInputErrorOnChange: true,
    initialValues: {
      username:
        cookies[PortalContants.CookieKeys.Authentication.RememberMe] &&
        cookies[PortalContants.CookieKeys.Authentication.UserName]
          ? cookies[PortalContants.CookieKeys.Authentication.UserName]
          : "",
      password: "",
      remember:
        cookies[PortalContants.CookieKeys.Authentication.RememberMe] ?? false,
    },
    validate: {
      username: isNotEmpty("Username is required"),
      password: isNotEmpty("Password is required"),
    },
  });

  const handleSubmit = form.onSubmit((values) => {
    setLoading(true);
    const api = getUser();
    api
      .postApiUserAuthenticate({
        userName: values.username,
        password: values.password,
      })
      .then((response) => {
        const authenticated = response && response.token;
        if (authenticated) {
          if (values.remember) {
            const expires = dayjs().add(30, "day").toDate();
            setCookie(
              PortalContants.CookieKeys.Authentication.UserName,
              values.username,
              { path: "/", expires },
            );
            setCookie(
              PortalContants.CookieKeys.Authentication.RememberMe,
              values.remember,
              { path: "/", expires },
            );
          } else {
            removeCookie(PortalContants.CookieKeys.Authentication.UserName);
            removeCookie(PortalContants.CookieKeys.Authentication.RememberMe);
          }

          signin(response);
        }
      })
      .catch((error: AxiosError<ServiceError>) => {
        if (error?.response?.data?.errorCode) {
          form.resetField("password");
          form.setFieldError("password", error.response.data.errorCode);
        }
      })
      .finally(() => {
        setLoading(false);
      });
  });

  return (
    <Flex className={classnames.screen}>
      <Paper radius="lg" shadow="xl" className={classnames.root}>
        <Stack align="stretch" gap={0}>
          <Stack align="center">
            <PortalLogo />
            <Text className={classnames.brand}>IMB-P</Text>
            <Text className={classnames.slogan} c="dimmed">
              Internet & Mobile Banking Portal
            </Text>
          </Stack>

          <Divider
            my={32}
            label={
              <Group justify="center" gap={8}>
                <LanguageSwitcher onChange={() => form.reset()} />
                <ColorSchemeSwitcher />
              </Group>
            }
          />

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

              <Group justify="space-between">
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
              </Group>
            </Stack>
          </form>
        </Stack>
      </Paper>
    </Flex>
  );
};

export default Auth;
