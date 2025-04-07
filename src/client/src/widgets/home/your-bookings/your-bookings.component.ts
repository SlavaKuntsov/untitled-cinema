import { JsonPipe } from "@angular/common";
import { Component, effect, inject, signal } from "@angular/core";
import {
  Booking,
  BookingHistoryPaginationPayload,
  BookingService,
} from "../../../entities/booking";
import { IError } from "../../../entities/error";
import { PaginationWrapper } from "../../../entities/movies";
import { UserService } from "../../../entities/users";

@Component({
  selector: "app-your-bookings",
  imports: [JsonPipe],
  templateUrl: "./your-bookings.component.html",
  styleUrl: "./your-bookings.component.scss",
})
export class YourBookingsComponent {
  userService = inject(UserService);
  bookingService = inject(BookingService);

  bookingHistory = signal<Booking[]>([]);

  payload = signal<BookingHistoryPaginationPayload>({
    userId: this.userService.user()?.id!,
    limit: 3,
    offset: 1,
    filters: [],
    filterValues: [],
    sortBy: "title",
    sortDirection: "asc",
    date: "",
  });

  constructor() {
    effect(() => {
      if (this.userService.user()) {

				this.payload.update()

        this.bookingService.getHistory(this.payload()).subscribe({
          next: (res: PaginationWrapper<Booking>) => {
            this.bookingHistory.set(res.items);
            // this.movie = res;
          },
          error: (error: IError) => {
            // this.errorText = "Hall not found :(";
          },
        });
      }
    });
  }
}
