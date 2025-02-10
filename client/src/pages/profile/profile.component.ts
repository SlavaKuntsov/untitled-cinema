import { Component, inject } from "@angular/core";
import { AuthService } from "../auth/api/auth.service";

@Component({
  selector: "app-profile",
  imports: [],
  templateUrl: "./profile.component.html",
  styleUrl: "./profile.component.scss",
})
export class ProfileComponent {
  authService = inject(AuthService);

  logout() {
    this.authService.logout().subscribe();
  }
}
