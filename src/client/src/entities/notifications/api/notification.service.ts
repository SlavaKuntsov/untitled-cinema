import { HttpClient } from "@angular/common/http";
import { inject, Injectable, Signal, signal } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { Observable, tap } from "rxjs";
import { environment } from "../../../environments/environment";
import { ToastService, ToastStatus } from "../../toast";
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
        this.notifications.update((notifications) => [
          ...notifications,
          message,
        ]);

        this.toastService.showToast(ToastStatus.Success, message.message);
      },
    );
  }

  stopConnection() {
    if (this.hubConnection && this.isConnected) {
      this.hubConnection.stop().then(() => {
        this.isConnected = false;
      });
    }
  }

  send(message: string): Observable<void> {
    return this.http
      .post<void>(
        `${environment.userBaseUrl}/notifications`,
        JSON.stringify(message),
        {
          headers: { "Content-Type": "application/json" },
          withCredentials: true,
        },
      )
      .pipe(tap(() => {}));
  }

  get(): Observable<CustomNotification[]> {
    return this.http
      .get<CustomNotification[]>(`${environment.userBaseUrl}/notifications`, {
        withCredentials: true,
      })
      .pipe(
        tap((res) => {
          this.notifications.set(res);
        }),
      );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.userBaseUrl}/notifications/${id}`,
      {
        withCredentials: true,
      },
    );
  }
}
