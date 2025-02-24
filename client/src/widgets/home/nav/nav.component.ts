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
import { ErrorService } from "../../../entities/error";
import { IError } from "../../../entities/error/model/error";
import { NotificationService } from "../../../entities/notifications/api/notification.service";
import { CustomNotification } from "../../../entities/notifications/model/customNotification";
import { ToastService, ToastStatus } from "../../../entities/toast";
import { User } from "../../../entities/users";
import { AuthService } from "../../../entities/users/api/auth.service";
import { UserService } from "../../../entities/users/api/user.service";
import { getChildRoutes } from "../../../shared/lib/model/get-child-routes";
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
  isUserLoaded: boolean = false;

  notifications: Signal<CustomNotification[]> =
    this.notificationService.notifications;

  auth = AUTH;
  account = ACCOUNT;

  isLoad: boolean = false;
  isNotificationsLoaded: boolean = false;

  constructor() {
    this.routes = getChildRoutes(this.router, "");

    effect(() => {
			if(this.user !== null){
				this.isUserLoaded = true;
			}
			else{
				this.isUserLoaded = false
			}
      if (this.user() && !this.isNotificationsLoaded) {
        this.notificationService.get().subscribe();

        this.isNotificationsLoaded = true;
      }
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
