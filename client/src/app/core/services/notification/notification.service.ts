import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";

@Injectable({
  providedIn: "root",
})
export class NotificationService {
  private hubConnection!: signalR.HubConnection;
  private notificationsSubject = new BehaviorSubject<string[]>([]);

  public notifications$ = this.notificationsSubject.asObservable();

  constructor() {}

  public startConnection() {
    const token = localStorage.getItem("yummy-apple");

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7001/notifications`, {
        accessTokenFactory: () => token ?? "",
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log("SignalR Connected"))
      .catch((err) => console.error("Error connecting to SignalR:", err));

    this.hubConnection.on("ReceiveNotification", (message: string) => {
      console.log("New notification:", message);
      this.notificationsSubject.next([
        ...this.notificationsSubject.value,
        message,
      ]);
    });
  }

  public stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().then(() => console.log("SignalR Disconnected"));
    }
  }
}
