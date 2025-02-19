import { CommonModule, DatePipe } from "@angular/common";
import { Component, HostListener, inject, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { format } from "date-fns";
import { ButtonModule } from "primeng/button";
import { DatePickerModule } from "primeng/datepicker";
import { Dialog } from "primeng/dialog";
import { ProgressSpinner } from "primeng/progressspinner";
import { ErrorService } from "../../../app/core/services/error/api/error.service";
import { ToastService, ToastStatus } from "../../../app/core/services/toast";
import { IError } from "../../../entities/error/model/error";
import { MoviesService } from "../../../entities/movies/api/movies.service";
import { Movie } from "../../../entities/movies/model/movie";
import { SessionService } from "../../../entities/sessions/api/session.service";
import { SessionsPaginationPayload } from "../../../entities/sessions/model/pagination";
import { Session } from "../../../entities/sessions/model/session";
import { AuthService } from "../../../entities/users/api/auth.service";
import { Base64ToImagePipe } from "../../../shared/lib/pipes/base64-to-image.pipe";
import { MinutesToHoursPipe } from "../../../shared/lib/pipes/minutes-to-hours.pipe";
import { TransformDatePipe } from "../../../shared/lib/pipes/transform-date.pipe";
import { ButtonComponent } from "../../../shared/ui/components/button/button.component";
import { ExtractTimePipe } from "../../../shared/lib/pipes/extract-time.pipe";

@Component({
  selector: "app-movie",
  imports: [
    DatePickerModule,
    CommonModule,
    Base64ToImagePipe,
    ProgressSpinner,
    MinutesToHoursPipe,
    ButtonComponent,
    FormsModule,
    TransformDatePipe,
    ButtonModule,
    Dialog,
		RouterLink,
		ExtractTimePipe
  ],
  templateUrl: "./movie.component.html",
  styleUrl: "./movie.component.scss",
})
export class MovieComponent {
  route = inject(ActivatedRoute);
  movieService = inject(MoviesService);
  toastService = inject(ToastService);
  sessionService = inject(SessionService);
  authService = inject(AuthService);
  errorService = inject(ErrorService);

  isDatepickerOpen = signal<boolean>(false);
  selectedDate: Date | null = null;
  isFilterDialogVisible: boolean = false;

  dateNow: Date = new Date();

  movieId!: string;
  movie: Movie | null = null;

  sessions: Session[] | null = null;

	errorText: string | null = null

  constructor() {
    this.movieId = this.route.snapshot.paramMap.get("movieId")!;

    this.movieService.getMovie(this.movieId).subscribe({
      next: (res: Movie) => {
        this.movie = res;
      },
      error: (error: IError) => {
        // const errorMessage = this.errorService.getErrorMessage(error);
        // this.toastService.showToast(ToastStatus.Error, errorMessage);
				this.errorText = 'Movie not found :('
      },
    });
  }

  selectSession() {
    this.isFilterDialogVisible = true;

    const payload: SessionsPaginationPayload = {
      limit: 100,
      offset: 1,
      date: format(this.dateNow, "dd-MM-yyyy"),
      hall: "",
    };

    this.sessionService.get(payload).subscribe({
      next: (res: Session[]) => {
        this.sessions = res;
      },
      error: (error: IError) => {
        // const errorMessage = this.errorService.getErrorMessage(error);
        // this.toastService.showToast(ToastStatus.Error, errorMessage);
      },
    });
  }

  onDatepickerChangeInDialog(date: Date) {
    if (date && date.getDate() !== this.dateNow.getDate()) {
      console.log("Выбранная дата отличается от сегодняшней:", date);
    } else {
    }

    this.isDatepickerOpen.set(false);
  }

  @HostListener("document:click", ["$event"])
  onClick(event: MouseEvent) {
    const datepickerContainer = document.querySelector(".custom-datepicker");
    const openButton = document.querySelector(".open-datepicker-button");

    const clickInsideDatepicker = datepickerContainer?.contains(
      event.target as Node,
    );
    const clickInsideOpenButton = openButton?.contains(event.target as Node);

    if (!clickInsideDatepicker && !clickInsideOpenButton) {
      this.isDatepickerOpen.set(false);
    }
  }
}
