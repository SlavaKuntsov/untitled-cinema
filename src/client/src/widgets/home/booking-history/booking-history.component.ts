import { Component, effect, inject, signal } from "@angular/core";
import { IError } from "../../../entities/error";
import { Movie } from "../../../entities/movies";
import { UserService } from "../../../entities/users";
import { Booking, BookingService } from "../../../entities/booking";
import { JsonPipe } from "@angular/common";

@Component({
  selector: "app-booking-history",
  imports: [
		JsonPipe
	],
  templateUrl: "./booking-history.component.html",
  styleUrl: "./booking-history.component.scss",
})
export class BookingHistoryComponent {
	userService = inject(UserService);
	bookingService = inject(BookingService);

	bookingHistory = signal<Booking[]>([])

  constructor() {
    effect(() => {
      if (this.userService.user()) {
        this.bookingService.getHistory(this.userService.user()?.id!).subscribe({
          next: (res: Booking[]) => {
						this.bookingHistory.set(res)
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
