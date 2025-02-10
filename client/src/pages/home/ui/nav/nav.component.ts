import { Component, inject } from "@angular/core";
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
} from "@angular/router";
import { Subscription } from "rxjs";
import { AUTH, PROFILE } from "../../../../shared/router/routes";
import { AuthService } from "../../../auth/api/auth.service";
import { CommonModule } from "@angular/common";

@Component({
  selector: "app-nav",
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: "./nav.component.html",
  styleUrl: "./nav.component.scss",
})
export class NavComponent {
  router = inject(Router);
  route = inject(ActivatedRoute);
  authService = inject(AuthService);
  userSubscription!: Subscription;

  isUser = this.authService.isUser$;
  routes: { route: string; title: string }[] = [];

  auth = AUTH;
  profile = PROFILE;

  constructor() {
    this.getChildRoutes();
  }

  private getChildRoutes() {
    const rootRoute = this.router.config.find((r) => r.path === "");
    if (rootRoute && rootRoute.children) {
      this.routes = rootRoute.children
        .filter((r) => r.path && r.data!["isVisible"])
        .map((r) => ({
          route: "/" + r.path,
          title: r.data!["name"],
        }));
    }
  }
}
