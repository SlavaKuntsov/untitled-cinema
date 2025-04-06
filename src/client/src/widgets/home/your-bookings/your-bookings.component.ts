import { Component, effect, inject, signal } from "@angular/core";
import { Booking, BookingService } from "../../../entities/booking";
import { IError } from "../../../entities/error";
import { UserService } from "../../../entities/users";
import { JsonPipe } from "@angular/common";

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

  constructor() {
    effect(() => {
      if (this.userService.user()) {
        this.bookingService.getHistory(this.userService.user()?.id!, true).subscribe({
          next: (res: Booking[]) => {
            this.bookingHistory.set(res);
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
