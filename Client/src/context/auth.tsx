import type { SafeUser } from "@/schemas/user.ts";

type Auth = {
  isAuthenticated: true;
  user: SafeUser;
};
type UnAuth = {
  isAuthenticated: false;
  user: null;
};

export type AuthContext = (Auth | UnAuth) & {
  setUser: (user: SafeUser | null) => void;
};

export const auth: AuthContext = {
  isAuthenticated: false,
  user: null,
  setUser: (user: SafeUser | null) => {
    auth.user = user;
    auth.isAuthenticated = !!user;
  },
};
