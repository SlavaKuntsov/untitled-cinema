import { CommonModule } from "@angular/common";
import {
  Component,
  effect,
  HostListener,
  inject,
  signal,
  Signal,
} from "@angular/core";
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
} from "@angular/router";
import { BadgeModule } from "primeng/badge";
import { Message } from "primeng/message";
import { ErrorService } from "../../../app/core/services/error/api/error.service";
import { ToastService, ToastStatus } from "../../../app/core/services/toast";
import { IError } from "../../../entities/error/model/error";
import { NotificationService } from "../../../entities/notifications/api/notification.service";
import { CustomNotification } from "../../../entities/notifications/model/customNotification";
import { User } from "../../../entities/users";
import { AuthService } from "../../../entities/users/api/auth.service";
import { UserService } from "../../../entities/users/api/user.service";
import { getChildRoutes } from "../../../shared/model/getChildRoutes";
import { ACCOUNT, AUTH } from "../../../shared/router/routes";
import { DividerComponent } from "../../../shared/ui/components/divider/divider.component";

@Component({
  selector: "app-nav",
  imports: [
    RouterLink,
    RouterLinkActive,
    CommonModule,
    DividerComponent,
    BadgeModule,
    Message,
  ],
  templateUrl: "./nav.component.html",
  styleUrl: "./nav.component.scss",
})
export class NavComponent {
  router = inject(Router);
  route = inject(ActivatedRoute);
  userService = inject(UserService);
  notificationService = inject(NotificationService);
  toastService = inject(ToastService);
  authService = inject(AuthService);
  errorService = inject(ErrorService);

  isNotifocationsOpen = signal<boolean>(false);

  routes: { route: string; title: string }[] = [];

  user: Signal<User | null> = this.userService.user;
	
  notifications: Signal<CustomNotification[]> =
    this.notificationService.notifications;

  auth = AUTH;
  account = ACCOUNT;

  isLoad: boolean = false;

  constructor() {
    this.routes = getChildRoutes(this.router, "");

    this.notificationService.get().subscribe();

    effect(() => {
      console.log("NAV");
      console.log(this.notifications());
    });
  }

  counter = 0;
  send() {
    this.counter++;
    this.notificationService.send(this.counter.toString()).subscribe();
  }

  openNotifications() {
    this.isNotifocationsOpen.set(!this.isNotifocationsOpen());
  }

  deleteNotification(id: string) {
    this.notificationService.delete(id).subscribe({
      next: () => {
        console.log("Notification deleted:", id);
        this.notificationService.notifications.update((notifications) => {
          return notifications.filter((i) => i.id !== id);
        });
      },
      error: (error: IError) => {
        const errorMessage = this.errorService.getErrorMessage(error);
        this.toastService.showToast(ToastStatus.Error, errorMessage);
      },
    });
  }

  @HostListener("document:click", ["$event"])
  onClick(event: MouseEvent) {
    const datepickerContainer = document.querySelector(".notifications");
    const openButton = document.querySelector(".open-notifications-button");

    const clickInsideDatepicker = datepickerContainer?.contains(
      event.target as Node,
    );
    const clickInsideOpenButton = openButton?.contains(event.target as Node);

    if (!clickInsideDatepicker && !clickInsideOpenButton) {
      this.isNotifocationsOpen.set(false);
    }
  }
}
