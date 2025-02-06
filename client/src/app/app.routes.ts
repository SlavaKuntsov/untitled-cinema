import { Routes } from "@angular/router";
import { AuthComponent } from "../pages/auth/auth.component";
import { LoginComponent } from "../pages/auth/ui/login/login.component";
import { RegistrationComponent } from "../pages/auth/ui/registration/registration.component";
import { BookingHistoryComponent } from "../pages/booking-history/booking-history.component";
import { HomePageComponent } from "../pages/home/home-page.component";
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

export const routes: Routes = [
  {
    path: "",
    component: HomePageComponent,
    data: { name: "Home" },
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
