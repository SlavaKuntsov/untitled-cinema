import { Component, effect, inject, OnDestroy, Signal } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { ButtonModule } from "primeng/button";
import { ToastModule } from "primeng/toast";
import { IError } from "../entities/error/model/error";
import { NotificationService } from "../entities/notifications/api/notification.service";
import { User } from "../entities/users";
import { AuthService } from "../entities/users/api/auth.service";
import { UserService } from "../entities/users/api/user.service";
import { ErrorService } from "./core/services/error/api/error.service";
import { ToastService, ToastStatus } from "./core/services/toast";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, ToastModule, ButtonModule],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent implements OnDestroy {
  authService = inject(AuthService);
  toastService = inject(ToastService);
  userService = inject(UserService);
  errorService = inject(ErrorService);
  notificationService = inject(NotificationService);

  isUser = false;

  user: Signal<User | null> = this.userService.user;
  accessTokenExist: Signal<boolean> = this.authService.accessTokenExist;

  constructor() {
    console.log("START");
    console.log(this.authService.accessToken());

    this.authService.authorize().subscribe({
      error: (error: IError) => {
        const errorMessage = this.errorService.getErrorMessage(error);
        this.toastService.showToast(ToastStatus.Error, errorMessage);
      },
    });

    effect(() => {

      if (this.userService.user() != null) {
        console.log("User authenticated, starting notifications...");
        this.notificationService.startConnection();
      } 
			else {
        console.log("User logged out, stopping notifications...");
        this.notificationService.stopConnection();
      }
    });
  }

  ngOnDestroy() {
    this.notificationService.stopConnection();
  }
}
