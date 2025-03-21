import { CommonModule } from "@angular/common";
import {
  Component,
  HostListener,
  inject,
  signal,
  WritableSignal,
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { format } from "date-fns";
import { ButtonModule } from "primeng/button";
import { DatePickerModule } from "primeng/datepicker";
import { Dialog } from "primeng/dialog";
import { ProgressSpinner } from "primeng/progressspinner";
import { ErrorService } from "../../../entities/error";
import { IError } from "../../../entities/error/model/error";
import { MoviesService } from "../../../entities/movies/api/movies.service";
import { Movie } from "../../../entities/movies/model/movie";
import { SessionService } from "../../../entities/sessions/api/session.service";
import { SessionsPaginationPayload } from "../../../entities/sessions/model/pagination";
import { Session } from "../../../entities/sessions/model/session";
import { ToastService } from "../../../entities/toast";
import { AuthService } from "../../../entities/users/api/auth.service";
import { Base64ToImagePipe } from "../../../shared/lib/pipes/base64-to-image.pipe";
import { ExtractTimePipe } from "../../../shared/lib/pipes/extract-time.pipe";
import { MinutesToHoursPipe } from "../../../shared/lib/pipes/minutes-to-hours.pipe";
import { TransformDatePipe } from "../../../shared/lib/pipes/transform-date.pipe";
import { ButtonComponent } from "../../../shared/ui/components/button/button.component";

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
    ExtractTimePipe,
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
  selectedMovieId: WritableSignal<string | null> =
    this.movieService.selectedMovieId;
  selectedSessionId: WritableSignal<string | null> =
    this.sessionService.selectedSessionId;

  sessions: Session[] | null = null;

  movieErrorText: string | null = null;
  sessionsErrorText: string | null = null;

  constructor() {
    this.movieId = this.route.snapshot.paramMap.get("movieId")!;

    this.movieService.getMovie(this.movieId).subscribe({
      next: (res: Movie) => {
        this.movie = res;
      },
      error: (error: IError) => {
        this.movieErrorText = "Movie not found :(";
      },
    });
  }

  buyTicket() {
    this.isFilterDialogVisible = true;

    this.fetchSessions(this.dateNow);
  }

  selectSession(session: string) {
    this.selectedMovieId.set(this.movieId);
    this.selectedSessionId.set(session);
  }

  private fetchSessions(date: Date) {
    const payload: SessionsPaginationPayload = {
      limit: 100,
      offset: 1,
      date: format(date, "dd-MM-yyyy"),
      movie: this.movieId,
      hall: "",
    };

    this.sessionService.get(payload).subscribe({
      next: (res: Session[]) => {
        this.sessions = res;
      },
      error: (error: IError) => {
        this.sessionsErrorText = "Session not found :(";
      },
    });
  }

  onDatepickerChangeInDialog(date: Date) {
    if (date && date.getDate() !== this.dateNow.getDate()) {
      this.fetchSessions(date);
    } else {
      this.fetchSessions(this.dateNow);
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
