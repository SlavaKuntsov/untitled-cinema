import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { SessionSeats } from "../model/booking";

@Injectable({
  providedIn: "root",
})
export class BookingService {
  private http = inject(HttpClient);

	// cheque: 

  getSeats(payload: string): Observable<SessionSeats> {
    return this.http.get<SessionSeats>(
      `${environment.bookingBaseUrl}/bookingsSeats/reserved/session/${payload}`,
    );
  }
}