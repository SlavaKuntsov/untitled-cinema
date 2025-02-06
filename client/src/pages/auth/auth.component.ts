import { Component, inject } from "@angular/core";
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from "@angular/router";
import { AUTH } from "../../shared/router/routes";

@Component({
  selector: "app-auth",
  imports: [RouterOutlet, RouterLinkActive, RouterLink],
  templateUrl: "./auth.component.html",
  styleUrl: "./auth.component.scss",
})
export class AuthComponent {
  routes: { route: string; title: string }[] = [];

  constructor() {
    this.getChildRoutes();
  }

  auth = AUTH;

  router = inject(Router);
  route = inject(ActivatedRoute);

  private getChildRoutes() {
    const rootRoute = this.router.config.find((r) => r.path === "auth");
    if (rootRoute && rootRoute.children) {
      this.routes = rootRoute.children
        .filter((r) => r.path)
        .map((r) => ({
          route: "/auth/" + r.path,
          title: r.data!["name"],
        }));
    }
  }
}
