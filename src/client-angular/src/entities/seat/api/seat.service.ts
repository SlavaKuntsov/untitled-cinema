import { HttpClient } from "@angular/common/http";
import { inject, Injectable, signal, Signal } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { Observable } from "rxjs";
import { Seat } from "../../../entities/seat";
import { environment } from "../../../environments/environment";
import { BookingService, SessionSeats } from "../../booking";
import { IError } from "../../error";
import { Hall } from "../../hall";
import { SessionService } from "../../sessions";
import { AuthService } from "../../users";
import { SeatType, SelectedSeat } from "../model/seat";

@Injectable({
  providedIn: "root",
})
export class SeatService {
  private http = inject(HttpClient);
  private hubConnection!: signalR.HubConnection;
  private isConnected = false;
  private authService = inject(AuthService);
  private bookingService = inject(BookingService);
  private sessionService = inject(SessionService);

  private accessToken: Signal<string | null> = this.authService.accessToken;

  selectedSessionId: Signal<string | null> =
    this.sessionService.selectedSessionId;

  // seats = ...

  // Сигнал только для seatsArray
  private _seatsArray = signal<number[][]>([]);
  public seatsArray = this._seatsArray.asReadonly();

  // Метод для обновления seatsArray
  updateSeatsArray(seats: number[][]): void {
    this._seatsArray.set(seats);
  }

  // Метод для обновления из полного объекта Hall
  updateFromHall(hall: Hall | null): void {
    this._seatsArray.set(hall?.seatsArray || []);
  }

  reservedSeats = signal<Seat[]>([]);

  getSelectedSeat(
    sessionId: string,
    row: number,
    column: number,
  ): Observable<SelectedSeat> {
    return this.http.get<SelectedSeat>(
      `${environment.movieBaseUrl}/seats/${sessionId}/${row}/${column}`,
    );
  }

  getSeatType(payload: string): Observable<SeatType[]> {
    return this.http.get<SeatType[]>(
      `${environment.movieBaseUrl}/seats/types/hall/${payload}`,
    );
  }

  startConnection(sessionId: string) {
    if (this.isConnected) return;
    // if (!this.userService.user) return;

    this.isConnected = true;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.bookingBaseUrl}/seatsHub`, {
        accessTokenFactory: () => this.accessToken() ?? "",
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log("SignalR Seats Connected");
        this.hubConnection.invoke("JoinSession", sessionId);
      })
      .catch((err) => {
        console.error("Error connecting to SignalR:", err);
        this.isConnected = false;
      });

    this.hubConnection.on("SeatChanged", (message: any) => {
      console.log("NEW SEATS UPDATE", message);

      // TODO - заменить запросом в бд
      // this.reservedSeats.update((currentSeats) => {
      //   // Проверяем, есть ли уже место с таким id в массиве
      //   const seatIndex = currentSeats.findIndex(
      //     (seat) => seat.id === message.seat.id,
      //   );

      //   if (seatIndex === -1) {
      //     // Места нет - добавляем
      //     const newSeat: Seat = {
      //       id: message.seat.id,
      //       row: message.seat.row, // предполагая, что message содержит эти поля
      //       column: message.seat.column,
      //     };
      //     return [...currentSeats, newSeat];
      //   } else {
      //     // Место есть - удаляем
      //     return currentSeats.filter((seat) => seat.id !== message.seat.id);
      //   }
      // });

      // this.reservedSeats.set(() => )

      this.bookingService
        .getReservedSeats(this.selectedSessionId()!)
        .subscribe({
          next: (res: SessionSeats) => {
            this.reservedSeats.set(res.reservedSeats!);
          },
          error: (error: IError) => {
            // this.errorText = "Hall not found :(";
          },
        });
    });
  }

  stopConnection(sessionId: string) {
    if (this.hubConnection && this.isConnected) {
      this.hubConnection.stop().then(() => {
        this.isConnected = false;
      });

      this.hubConnection.invoke("LeaveSession", sessionId);
    }
  }
}
