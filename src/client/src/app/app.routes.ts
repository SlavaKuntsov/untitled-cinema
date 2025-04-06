import { Routes } from "@angular/router";
import { AuthComponent } from "../pages/auth/auth.component";
import { HomeComponent } from "../pages/home/home.component";
import { NotFoundPageComponent } from "../pages/not-found/not-found-page.component";
import {
  ACCOUNT,
  AUTH,
  HISTORY,
  LOGIN,
  POSTER,
  PROFILE,
  REGISTRATION,
	YOUR_BOOKINGS,
} from "../shared/router/routes";
import { LoginComponent } from "../widgets/auth/login/login.component";
import { RegistrationComponent } from "../widgets/auth/registration/registration.component";
import { AccountComponent } from "../widgets/home/account/account.component";
import { BookingHistoryComponent } from "../widgets/home/booking-history/booking-history.component";
import { HallComponent } from "../widgets/home/hall/hall.component";
import { MovieComponent } from "../widgets/home/movie/movie.component";
import { PosterComponent } from "../widgets/home/poster/poster.component";
import { ProfileComponent } from "../widgets/home/profile/profile.component";
import { canActivateAuth } from "./core/guards/auth/access.guard";
import { canActivateHome } from "./core/guards/home/home.guad";
import { YourBookingsComponent } from "../widgets/home/your-bookings/your-bookings.component";

export const routes: Routes = [
  {
    path: "",
    component: HomeComponent,
    canActivate: [canActivateHome],
    children: [
      {
        path: POSTER,
        data: { name: "Poster", isVisible: true },
        children: [
          {
            path: "",
            component: PosterComponent,
          },
          {
            path: ":movieId",
            component: MovieComponent,
          },
        ],
      },
      {
        path: "hall/:hallId",
        component: HallComponent,
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
          {
            path: YOUR_BOOKINGS,
            component: YourBookingsComponent,
            data: { name: "Your Bookings", isVisible: true },
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
