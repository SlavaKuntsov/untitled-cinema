import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { PaginationWrapper } from "../../movies";
import {
  Booking,
  BookingDto,
  BookingHistoryPaginationPayload,
  SessionSeats,
} from "../model/booking";

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

  createBooking(payload: BookingDto) {
    return this.http.post(`${environment.bookingBaseUrl}/bookings`, payload, {
      withCredentials: true,
      responseType: "json",
    });
  }

  getHistory(
    payload: BookingHistoryPaginationPayload,
  ): Observable<PaginationWrapper<Booking>> {
    let params = new HttpParams()
      .set("UserId", payload.userId.toString())
      .set("Limit", payload.limit.toString())
      .set("Offset", payload.offset.toString())
      .set("SortBy", payload.sortBy)
      .set("SortDirection", payload.sortDirection)
      .set("Date", payload.date);

    if (payload.filters && payload.filterValues) {
      payload.filters.forEach((filter, index) => {
        params = params
          .append("Filter", filter)
          .append("FilterValue", payload.filterValues[index]);
      });
    }

    return this.http.get<PaginationWrapper<Booking>>(
      `${environment.bookingBaseUrl}/bookings/history`,
      {
        params,
      },
    );
  }
}
