import { CommonModule } from "@angular/common";
import {
  Component,
  effect,
  HostListener,
  inject,
  Signal,
  signal,
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ButtonModule } from "primeng/button";
import { ChipModule } from "primeng/chip";
import { DatePickerModule } from "primeng/datepicker";
import { Dialog } from "primeng/dialog";
import { PaginatorModule, PaginatorState } from "primeng/paginator";
import { SkeletonModule } from "primeng/skeleton";
import { ErrorService } from "../../../../app/core/services/error/api/error.service";
import { ToastService, ToastStatus } from "../../../../app/core/services/toast";
import { IError } from "../../../../entities/error/model/error";
import { MoviesService } from "../../../../entities/movies/api/movies.service";
import { Movie } from "../../../../entities/movies/model/movie";
import {
  PaginationPayload,
  PaginationWrapper,
} from "../../../../entities/movies/model/pagination";
import { Base64ToImagePipe } from "../../../../shared/lib/pipes/base64-to-image.pipe";
import { MinutesToHoursPipe } from "../../../../shared/lib/pipes/minutes-to-hours.pipe";

@Component({
  selector: "app-poster",
  imports: [
    DatePickerModule,
    FormsModule,
    Dialog,
    ButtonModule,
    SkeletonModule,
    CommonModule,
    PaginatorModule,
    ChipModule,
    Base64ToImagePipe,
    MinutesToHoursPipe,
  ],
  templateUrl: "./poster.component.html",
  styleUrl: "./poster.component.scss",
})
export class PosterComponent {
  moviesService = inject(MoviesService);
  toastService = inject(ToastService);
  errorService = inject(ErrorService);

  isDatepickerOpen = signal<boolean>(false);
  selectedDate: Date | null = null;
  isFilterDialogVisible: boolean = false;

  readonly DATE_TODAY: string = "Today";
  dateHeading: typeof this.DATE_TODAY | string = this.DATE_TODAY;

  dateNow: Date = new Date();

  total = signal<number>(0);

  movies: Signal<PaginationWrapper<Movie> | null> = this.moviesService.movies;
  payload = signal<PaginationPayload>({
    limit: 1,
    offset: 1,
    filter: "",
    filterValue: "",
    sortBy: "title",
    sortDirection: "asc",
  });

  constructor() {
    effect(() => {
      console.log("effect_________________");
      console.log(this.payload());

      this.moviesService.get(this.payload()).subscribe({
        next: (res: PaginationWrapper<Movie>) => {
          this.total.set(res.total);
        },
        error: (error: IError) => {
          const errorMessage = this.errorService.getErrorMessage(error);
          this.toastService.showToast(ToastStatus.Error, errorMessage);
        },
      });
    });
  }

  first: number = 0;

  onPageChange(event: PaginatorState) {
    this.first = event.first!;

    this.payload.update((val) => ({
      ...val,
      limit: event.rows!,
      offset: event.page! + 1,
    }));
  }

  chooseFilters() {
    this.isFilterDialogVisible = true;
  }

  openDatepicker() {
    this.isDatepickerOpen.set(!this.isDatepickerOpen());
  }

  setDateToday() {
		this.dateHeading = this.DATE_TODAY
  }

  onDatepickerChange(date: Date) {
    const today = new Date();

    if (date && date.getDate() !== today.getDate()) {
      console.log("Выбранная дата отличается от сегодняшней:", date);

      const formatter = new Intl.DateTimeFormat("en-US", {
        month: "long",
        day: "numeric",
      });

      const formattedDate = formatter.format(date).replace(" ", ", ");
      this.dateHeading = formattedDate;
    } else {
      this.dateHeading = "Today";
      console.log("Выбрана сегодняшняя дата");
    }

    this.isDatepickerOpen.set(false);
  }

  counterArray(n: number): any[] {
    return Array(n);
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
