import { Component, effect, inject, Signal } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { ButtonModule } from "primeng/button";
import { ToastModule } from "primeng/toast";
import { IError } from "../entities/error/model/error";
import { AuthService } from "../entities/users/api/auth.service";
import { ErrorService } from "./core/services/error/api/error.service";
import { NotificationService } from "./core/services/notification/notification.service";
import { ToastService, ToastStatus } from "./core/services/toast";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, ToastModule, ButtonModule],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  authService = inject(AuthService);
  toastService = inject(ToastService);
  errorService = inject(ErrorService);
  notificationService = inject(NotificationService);

  notifications: string[] = [];
  isUser = false;

  accessTokenExist: Signal<boolean> = this.authService.accessTokenExist;

  constructor() {
    console.log("START");
    console.log(this.authService.accessToken());

    this.authService.authorize().subscribe({
      error: (error: IError) => {
        const errorMessage = this.errorService.getErrorMessage(error);
        // this.toastService.showToast(ToastStatus.Error, errorMessage);
      },
    });

    effect(() => {
      if (this.accessTokenExist()) {
        console.log("User authenticated, starting notifications...");
        // this.notificationService.startConnection();
      } else {
        console.log("User logged out, stopping notifications...");
        this.notificationService.stopConnection();
      }
    });

    // this.userSubscription = this.authService.isUser$.subscribe(
    //   (isAuthenticated) => {
    //     this.isUser = isAuthenticated;
    //     if (isAuthenticated) {
    //       console.log("User authenticated, starting notifications...");
    //       // this.notificationService.startConnection();
    //     } else {
    //       console.log("User logged out, stopping notifications...");
    //       this.notificationService.stopConnection();
    //     }
    //   },
    // );

    this.notificationService.notifications$.subscribe((messages) => {
      if (messages.length > 0) {
        const latestMessage = messages[messages.length - 1];
        console.log("Получено сообщение:", latestMessage);
        this.toastService.showToast(ToastStatus.Success, latestMessage);
      }
    });
  }

  ngOnDestroy() {
    this.notificationService.stopConnection();
  }
}
