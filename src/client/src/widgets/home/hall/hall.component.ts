import { CommonModule, CurrencyPipe } from "@angular/common";
import { Component, effect, inject, Signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { ButtonModule } from "primeng/button";
import { DatePickerModule } from "primeng/datepicker";
import { ProgressSpinner } from "primeng/progressspinner";
import { Tooltip } from "primeng/tooltip";
import { BookingService, Seat, SessionSeats } from "../../../entities/booking";
import { ErrorService } from "../../../entities/error";
import { IError } from "../../../entities/error/model/error";
import { Hall } from "../../../entities/hall";
import { HallService } from "../../../entities/hall/api/hall.service";
import { Movie } from "../../../entities/movies";
import { MoviesService } from "../../../entities/movies/api/movies.service";
import { SeatService, SeatType } from "../../../entities/seat";
import { Session } from "../../../entities/sessions";
import { SessionService } from "../../../entities/sessions/api/session.service";
import { ToastService } from "../../../entities/toast";
import { AuthService } from "../../../entities/users/api/auth.service";
import { Base64ToImagePipe } from "../../../shared/lib/pipes/base64-to-image.pipe";
import { ExtractDatePipe } from "../../../shared/lib/pipes/extract-date.pipe";
import { ExtractTimePipe } from "../../../shared/lib/pipes/extract-time.pipe";
import { MinutesToHoursPipe } from "../../../shared/lib/pipes/minutes-to-hours.pipe";
import { ButtonComponent } from "../../../shared/ui/components/button/button.component";

@Component({
  selector: "app-hall",
  imports: [
    DatePickerModule,
    CommonModule,
    Base64ToImagePipe,
    ProgressSpinner,
    MinutesToHoursPipe,
    FormsModule,
    ButtonComponent,
    ButtonModule,
    ExtractTimePipe,
    ExtractDatePipe,
    RouterLink,
    Tooltip,
    CurrencyPipe,
  ],
  templateUrl: "./hall.component.html",
  styleUrl: "./hall.component.scss",
})
export class HallComponent {
  route = inject(ActivatedRoute);
  movieService = inject(MoviesService);
  hallService = inject(HallService);
  toastService = inject(ToastService);
  seatService = inject(SeatService);
  sessionService = inject(SessionService);
  authService = inject(AuthService);
  bookingService = inject(BookingService);
  errorService = inject(ErrorService);

  hallId!: string;
  hall: Hall | null = null;
  movie: Movie | null = null;
  session: Session | null = null;
  seatTypes: SeatType[] | null = null;
  reservedSeats: Seat[] = [];
  selectedMovieId: Signal<string | null> = this.movieService.selectedMovieId;
  selectedSessionId: Signal<string | null> =
    this.sessionService.selectedSessionId;

  errorText: string | null = null;

  constructor() {
    this.hallId = this.route.snapshot.paramMap.get("hallId")!;

    this.hallService.getHall(this.hallId).subscribe({
      next: (res: Hall) => {
        this.hall = res;
      },
      error: (error: IError) => {
        this.errorText = "Hall not found :(";
      },
    });

    this.seatService.get(this.hallId).subscribe({
      next: (res: SeatType[]) => {
        this.seatTypes = res.sort((a, b) => a.priceModifier - b.priceModifier);
      },
    });

    effect(() => {
      if (this.selectedMovieId() == null) {
        this.errorText = "Hall not found :(";
      }

      if (this.selectedMovieId()) {
        this.movieService.getMovie(this.selectedMovieId()!).subscribe({
          next: (res: Movie) => {
            this.movie = res;
          },
          error: (error: IError) => {
            this.errorText = "Hall not found :(";
          },
        });
      }
      if (this.selectedSessionId()) {
        this.sessionService.getSession(this.selectedSessionId()!).subscribe({
          next: (res: Session) => {
            this.session = res;
          },
          error: (error: IError) => {
            this.errorText = "Hall not found :(";
          },
        });

        this.bookingService.getSeats(this.selectedSessionId()!).subscribe({
          next: (res: SessionSeats) => {
            this.reservedSeats = res.reservedSeats!;
          },
          error: (error: IError) => {
            this.errorText = "Hall not found :(";
          },
        });
      }
    });
  }

  isReserved(rowIndex: number, seatIndex: number): boolean {
    const rowNumber = rowIndex + 1;
    const seatNumber = seatIndex + 1; 
    return this.reservedSeats.some(
      (seat) => seat.row === rowNumber && seat.column === seatNumber,
    );
  }
}
