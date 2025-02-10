import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "../../../../pages/auth/api/auth.service";

export const canActivateAuth = () => {
  const authService = inject(AuthService);
  const isLoggedIn = authService.isUser;

  if (isLoggedIn) {
    return true;
  }

	// authService.logout()
  return inject(Router).navigate(["/auth"]);
};
