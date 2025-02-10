import { Routes } from "@angular/router";
import { AuthComponent } from "../pages/auth/auth.component";
import { LoginComponent } from "../pages/auth/ui/login/login.component";
import { RegistrationComponent } from "../pages/auth/ui/registration/registration.component";
import { BookingHistoryComponent } from "../pages/booking-history/booking-history.component";
import { HomeComponent } from "../pages/home/home.component";
import { NotFoundPageComponent } from "../pages/not-found/not-found-page.component";
import { PosterComponent } from "../pages/poster/poster.component";
import { ProfileComponent } from "../pages/profile/profile.component";
import {
  AUTH,
  HISTORY,
  LOGIN,
  POSTER,
  PROFILE,
  REGISTRATION,
} from "../shared/router/routes";
import { canActivateAuth } from "./core/guard/auth/access.guard";
import { canActivateHome } from "./core/guard/home/home.guad";

export const routes: Routes = [
  {
    path: "",
    component: HomeComponent,
    canActivate: [canActivateHome],
    children: [
      {
        path: POSTER,
        component: PosterComponent,
        data: { name: "Poster" },
      },
      {
        path: HISTORY,
        component: BookingHistoryComponent,
        data: { name: "Booking History" },
      },
    ],
  },
  {
    path: PROFILE,
    component: ProfileComponent,
    data: { name: "Profile" },
    canActivate: [canActivateAuth],
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
