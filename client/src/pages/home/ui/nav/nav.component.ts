import { Component, inject } from "@angular/core";
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
} from "@angular/router";
import { AUTH, PROFILE } from "../../../../shared/router/routes";
import { AuthService } from "../../../auth/api/auth.service";

@Component({
  selector: "app-nav",
  imports: [RouterLink, RouterLinkActive],
  templateUrl: "./nav.component.html",
  styleUrl: "./nav.component.scss",
})
export class NavComponent {
  router = inject(Router);
  route = inject(ActivatedRoute);
  authService = inject(AuthService);

  isLoggedIn = this.authService.isAuth;

  ngOnInit() {
    this.getChildRoutes();
  }

  routes: { route: string; title: string }[] = [];

  auth = AUTH;
  profile = PROFILE;

  private getChildRoutes() {
    const rootRoute = this.router.config.find((r) => r.path === "");
    if (rootRoute && rootRoute.children) {
      this.routes = rootRoute.children
        .filter((r) => r.path)
        .map((r) => ({
          route: "/" + r.path,
          title: r.data!["name"],
        }));
    }
  }
}
