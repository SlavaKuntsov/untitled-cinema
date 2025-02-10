import { inject } from "@angular/core";
import { AuthService } from "../../../../pages/auth/api/auth.service";

export const canActivateHome = () => {
  const isLoggedIn = inject(AuthService).isAuth;

  if (isLoggedIn) {
    return true;
  }

  return true;
};
