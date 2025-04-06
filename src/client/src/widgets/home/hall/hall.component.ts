import { CommonModule, CurrencyPipe } from "@angular/common";
import {
  Component,
  computed,
  effect,
  inject,
  OnDestroy,
  signal,
  Signal,
  WritableSignal,
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { DatePickerModule } from "primeng/datepicker";
import { ProgressSpinner } from "primeng/progressspinner";
import { Tooltip } from "primeng/tooltip";
import {
  Booking,
  BookingService,
  SessionSeats,
} from "../../../entities/booking";
import { ErrorService } from "../../../entities/error";
import { IError } from "../../../entities/error/model/error";
import { Hall } from "../../../entities/hall";
import { HallService } from "../../../entities/hall/api/hall.service";
import { Movie } from "../../../entities/movies";
import { MoviesService } from "../../../entities/movies/api/movies.service";
import {
  Seat,
  SeatService,
  SeatType,
  SelectedSeat,
} from "../../../entities/seat";
import { Session } from "../../../entities/sessions";
import { SessionService } from "../../../entities/sessions/api/session.service";
import { ToastService, ToastStatus } from "../../../entities/toast";
import { UserService } from "../../../entities/users";
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
    ConfirmDialogModule,
    CurrencyPipe,
  ],
  providers: [ConfirmationService],
  templateUrl: "./hall.component.html",
  styleUrl: "./hall.component.scss",
})
export class HallComponent implements OnDestroy {
  route = inject(ActivatedRoute);
  movieService = inject(MoviesService);
  hallService = inject(HallService);
  toastService = inject(ToastService);
  seatService = inject(SeatService);
  sessionService = inject(SessionService);
  bookingService = inject(BookingService);
  authService = inject(AuthService);
  userService = inject(UserService);
  errorService = inject(ErrorService);
  confirmationService = inject(ConfirmationService);

  hallId!: string;
  hall: Hall | null = null;
  seatsArray = this.seatService.seatsArray;
  movie: Movie | null = null;
  session: Session | null = null;
  seatTypes: SeatType[] | null = null;
  // reservedSeats: Seat[] = [];
  reservedSeats: WritableSignal<Seat[]> = this.seatService.reservedSeats;
  selectedSeats = signal<SelectedSeat[]>([]);
  selectedMovieId: Signal<string | null> = this.movieService.selectedMovieId;
  selectedSessionId: Signal<string | null> =
    this.sessionService.selectedSessionId;

  errorText: string | null = null;

  // В компоненте
  totalPrice = computed(() =>
    this.selectedSeats().reduce((sum, seat) => sum + seat.price, 0),
  );

  constructor() {
    this.hallId = this.route.snapshot.paramMap.get("hallId")!;

    this.hallService.getHall(this.hallId).subscribe({
      next: (res: Hall) => {
        this.hall = res;
        this.seatService.updateFromHall(res);
      },
      error: (error: IError) => {
        this.errorText = "Hall not found :(";
      },
    });

    this.seatService.getSeatType(this.hallId).subscribe({
      next: (res: SeatType[]) => {
        this.seatTypes = res.sort((a, b) => a.priceModifier - b.priceModifier);
      },
    });

    effect(() => {
      if (this.hallId != null && this.selectedSessionId()) {
        console.log("Starting change seats...");
        this.seatService.startConnection(this.selectedSessionId()!);
      }

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

        // this.bookingService
        //   .getAvailableSeats(this.selectedSessionId()!)
        //   .subscribe({
        //     next: (res: SessionSeats) => {
        //       this.reservedSeats = res.reservedSeats!;
        //     },
        //     error: (error: IError) => {
        //       this.errorText = "Hall not found :(";
        //     },
        //   });

        this.fetchReservedSeats();
      }
    });
  }

  fetchReservedSeats() {
    this.bookingService.getReservedSeats(this.selectedSessionId()!).subscribe({
      next: (res: SessionSeats) => {
        this.reservedSeats.set(res.reservedSeats!);
      },
      error: (error: IError) => {
        this.errorText = "Hall not found :(";
      },
    });
  }

  addToggleSeatSelection(row: number, column: number): void {
    row++;
    column++;
    this.seatService
      .getSelectedSeat(this.selectedSessionId()!, row, column)
      .subscribe({
        next: (res: SelectedSeat) => {
          // this.seatService.updateFromHall(res);

          this.selectedSeats.update((current) => {
            const existingIndex = current.findIndex(
              (s) => s.row === row && s.column === column,
            );

            if (existingIndex >= 0) {
              // Удаляем если уже есть
              return current.filter((_, i) => i !== existingIndex);
            } else {
              // Добавляем если нет
              return [...current, res];
            }
          });
        },
        error: (error: IError) => {
          this.errorText = "Hall not found :(";
        },
      });
  }

  removeToggleSeatSelection(id: string): void {
    this.selectedSeats.update((current) =>
      current.filter((seat) => seat.id !== id),
    );
  }

  booking(): void {
    if (!this.userService.user()) {
      this.confirmationService.confirm({
        header: "Login, please.",
        message: "Please confirm to proceed.",
        key: "booking",
        accept: () => {
          console.log("qwe");
        },
        reject: () => {},
      });
      return;
    }

    var payload: Booking = {
      sessionId: this.selectedSessionId()!,
      seats: this.selectedSeats(),
      userId: this.userService.user()?.id!,
    };

    this.bookingService.booking(payload).subscribe({
      next: () => {
        this.selectedSeats.update(() => []);
      },
      error: (error: IError) => {
        const errorMessage = this.errorService.getErrorMessage(error);
        this.toastService.showToast(ToastStatus.Error, errorMessage);
      },
    });
  }

  clearSelections(): void {
    this.selectedSeats.set([]);
  }

  isReserved(rowIndex: number, seatIndex: number): boolean {
    const rowNumber = rowIndex + 1;
    const seatNumber = seatIndex + 1;
    // console.log("isReserved", this.reservedSeats());
    return this.reservedSeats().some(
      (seat) => seat.row === rowNumber && seat.column === seatNumber,
    );
  }

  isSelected(rowIndex: number, seatIndex: number): boolean {
    return this.selectedSeats().some(
      (s) => s.row === rowIndex + 1 && s.column === seatIndex + 1,
    );
  }

  ngOnDestroy() {
    this.seatService.stopConnection(this.selectedSessionId()!);
  }
}
