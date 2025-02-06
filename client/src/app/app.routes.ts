import { Routes } from "@angular/router";
import { AuthComponent } from "../pages/auth/auth.component";
import { HomePageComponent } from "../pages/home-page/home-page.component";
import { NotFoundPageComponent } from "../pages/not-found-page/not-found-page.component";
import { PosterComponent } from "../pages/poster/poster.component";
import { ProfileComponent } from "../shared/ui/components/profile/profile.component";

export const routes: Routes = [
  {
    path: "",
    component: HomePageComponent,
  },
  {
    path: "auth",
    component: AuthComponent,
  },
  {
    path: "poster",
    component: PosterComponent,
  },
  {
    path: "profile",
    component: ProfileComponent,
  },
  {
    path: "**",
    component: NotFoundPageComponent,
  },
];
