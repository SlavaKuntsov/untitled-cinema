import { CommonModule } from "@angular/common";
import { Component, inject, Signal } from "@angular/core";
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
} from "@angular/router";
import { User } from "../../../../entities/users";
import { UserService } from "../../../../entities/users/api/user.service";
import { getChildRoutes } from "../../../../shared/model/getChildRoutes";
import { ACCOUNT, AUTH } from "../../../../shared/router/routes";

@Component({
  selector: "app-nav",
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: "./nav.component.html",
  styleUrl: "./nav.component.scss",
})
export class NavComponent {
  router = inject(Router);
  route = inject(ActivatedRoute);
  userService = inject(UserService);

  routes: { route: string; title: string }[] = [];

  user: Signal<User | null> = this.userService.user;

  auth = AUTH;
  account = ACCOUNT;

  isLoad: boolean = false;

  constructor() {
    this.routes = getChildRoutes(this.router, "");
  }
}
