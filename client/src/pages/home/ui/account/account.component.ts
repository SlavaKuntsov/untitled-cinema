import { CommonModule } from "@angular/common";
import { Component, inject, Signal } from "@angular/core";
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from "@angular/router";
import { AvatarModule } from "primeng/avatar";
import { AvatarGroupModule } from "primeng/avatargroup";
import { ProgressSpinner } from "primeng/progressspinner";
import { User } from "../../../../entities/users";
import { AuthService } from "../../../../entities/users/api/auth.service";
import { UserService } from "../../../../entities/users/api/user.service";
import { PROFILE } from "../../../../shared/router/routes";
import { DividerComponent } from "../../../../shared/ui/components/divider/divider.component";

@Component({
  selector: "app-account",
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    CommonModule,
    AvatarModule,
    AvatarGroupModule,
    ProgressSpinner,
		DividerComponent
  ],
  templateUrl: "./account.component.html",
  styleUrl: "./account.component.scss",
})
export class AccountComponent {
  authService = inject(AuthService);
  userService = inject(UserService);
  router = inject(Router);

  routes: { route: string; title: string }[] = [];

  user: Signal<User | null> = this.userService.user;

  constructor() {
    this.routes = [
      { route: PROFILE, title: "Profile" },
      { route: "bookings", title: "Your bookings" },
    ];
  }
}
