import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { ToastModule } from "primeng/toast";
import { Subscription } from "rxjs";
import { AuthService } from "../pages/auth/api/auth.service";
import { NotificationService } from "./core/services/notification/notification.service";
import { ToastService, ToastStatus } from "./core/services/toast";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, ToastModule],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  authService = inject(AuthService);
  toastService = inject(ToastService);
  notificationService = inject(NotificationService);
  private userSubscription!: Subscription;

  notifications: string[] = [];
  isUser = false;

  ngOnInit() {
    this.authService.authorize().subscribe((val) => console.log(val));

    this.userSubscription = this.authService.isUser$.subscribe(
      (isAuthenticated) => {
        this.isUser = isAuthenticated;
        if (isAuthenticated) {
          console.log("User authenticated, starting notifications...");
          // this.notificationService.startConnection();
        } else {
          console.log("User logged out, stopping notifications...");
          this.notificationService.stopConnection();
        }
      },
    );

    this.notificationService.notifications$.subscribe((messages) => {
      if (messages.length > 0) {
        const latestMessage = messages[messages.length - 1];
        console.log("Получено сообщение:", latestMessage);
        this.toastService.showToast(ToastStatus.Success, latestMessage);
      }
    });
  }

  ngOnDestroy() {
    this.userSubscription.unsubscribe();
    this.notificationService.stopConnection();
  }
}
