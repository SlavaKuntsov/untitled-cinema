import { Routes } from "@angular/router";
import { AuthComponent } from "../pages/auth/auth.component";
import { HomeComponent } from "../pages/home/home.component";
import { AccountComponent } from "../pages/home/ui/account/account.component";
import { PosterComponent } from "../pages/home/ui/poster/poster.component";
import { NotFoundPageComponent } from "../pages/not-found/not-found-page.component";
import {
  ACCOUNT,
  AUTH,
  HISTORY,
  LOGIN,
  POSTER,
  PROFILE,
  REGISTRATION,
} from "../shared/router/routes";
import { LoginComponent } from "../widgets/auth/login/login.component";
import { RegistrationComponent } from "../widgets/auth/registration/registration.component";
import { BookingHistoryComponent } from "../widgets/home/booking-history/booking-history.component";
import { ProfileComponent } from "../widgets/home/profile/profile.component";
import { canActivateAuth } from "./core/guards/auth/access.guard";
import { canActivateHome } from "./core/guards/home/home.guad";

export const routes: Routes = [
  {
    path: "",
    component: HomeComponent,
    canActivate: [canActivateHome],
    children: [
      {
        path: POSTER,
        component: PosterComponent,
        data: { name: "Poster", isVisible: true },
      },
      {
        path: HISTORY,
        component: BookingHistoryComponent,
        data: { name: "Booking History", isVisible: true },
      },
      {
        path: ACCOUNT,
        component: AccountComponent,
        data: { name: "Account", isVisible: false },
        canActivate: [canActivateAuth],
        children: [
          {
            path: "",
            redirectTo: PROFILE,
            pathMatch: "full",
          },
          {
            path: PROFILE,
            component: ProfileComponent,
            data: { name: "Profile", isVisible: true },
          },
        ],
      },
    ],
  },
  {
    path: AUTH,
    component: AuthComponent,
    data: { name: "Auth" },
    children: [
      {
        path: "",
        redirectTo: LOGIN,
        pathMatch: "full",
      },
      {
        path: LOGIN,
        component: LoginComponent,
        data: { name: "Login" },
      },
      {
        path: REGISTRATION,
        component: RegistrationComponent,
        data: { name: "Registration" },
      },
    ],
  },
  {
    path: "**",
    component: NotFoundPageComponent,
  },
];
