import { HttpClient } from "@angular/common/http";
import { inject, Injectable, Signal, signal } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { Observable, tap } from "rxjs";
import { ToastService, ToastStatus } from "../../../app/core/services/toast";
import { userBaseUrl } from "../../../shared/config/backend";
import { AuthService } from "../../users/api/auth.service";
import { CustomNotification } from "../model/customNotification";

@Injectable({
  providedIn: "root",
})
export class NotificationService {
  private http = inject(HttpClient);
  private hubConnection!: signalR.HubConnection;
  private isConnected = false;
  private authService = inject(AuthService);
  private toastService = inject(ToastService);

  notifications = signal<CustomNotification[]>([]);
  private accessToken: Signal<string | null> = this.authService.accessToken;

  startConnection() {
    if (this.isConnected) return;
    this.isConnected = true;

    console.log("------------START SIGNALR CONNECTION");

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7001/notify`, {
        accessTokenFactory: () => this.accessToken() ?? "",
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log("SignalR Connected"))
      .catch((err) => {
        console.error("Error connecting to SignalR:", err);
        this.isConnected = false;
      });

    this.hubConnection.on(
      "ReceiveNotification",
      (message: CustomNotification) => {
				console.log('UPDATE')
				console.log(this.notifications())
        this.notifications.update((notifications) => [
          ...notifications,
          message,
        ]);
				console.log(this.notifications())

        this.toastService.showToast(ToastStatus.Success, message.message);
      },
    );
  }

  stopConnection() {
    if (this.hubConnection && this.isConnected) {
      this.hubConnection.stop().then(() => {
        console.log("SignalR Disconnected");
        this.isConnected = false;
      });
    }
  }

  send(message: string): Observable<void> {
    return this.http
      .post<void>(`${userBaseUrl}/notifications`, JSON.stringify(message), {
        headers: { "Content-Type": "application/json" },
        withCredentials: true,
      })
      .pipe(tap(() => {}));
  }

  get(): Observable<CustomNotification[]> {
    return this.http
      .get<CustomNotification[]>(`${userBaseUrl}/notifications`, {
        withCredentials: true,
      })
      .pipe(
        tap((res) => {
          this.notifications.set(res);
        }),
      );
  }

  delete(id: string): Observable<void> {
    return this.http
      .delete<void>(`${userBaseUrl}/notifications/${id}`, {
        withCredentials: true,
      })
      .pipe(
        tap(() => {
          // Обновление состояния notifications происходит в компоненте
        }),
      );
  }
}
