import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { Booking, BookingDto, SessionSeats } from "../model/booking";

@Injectable({
  providedIn: "root",
})
export class BookingService {
  private http = inject(HttpClient);

  // cheque:

  getAvailableSeats(payload: string): Observable<SessionSeats> {
    return this.http.get<SessionSeats>(
      `${environment.bookingBaseUrl}/bookingsSeats/available/session/${payload}`,
    );
  }

  getReservedSeats(payload: string): Observable<SessionSeats> {
    return this.http.get<SessionSeats>(
      `${environment.bookingBaseUrl}/bookingsSeats/reserved/session/${payload}`,
    );
  }

  booking(payload: BookingDto) {
    return this.http.post(`${environment.bookingBaseUrl}/bookings`, payload, {
      withCredentials: true,
      responseType: "json",
    });
  }

  getHistory(userId: string): Observable<Booking[]> {
    return this.http.get<Booking[]>(
      `${environment.bookingBaseUrl}/bookings/history/${userId}`,
    );
  }
}
