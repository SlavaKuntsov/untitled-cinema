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
import { RouterLink } from "@angular/router";
import { format } from "date-fns";
import { ButtonModule } from "primeng/button";
import { ChipModule } from "primeng/chip";
import { DatePickerModule } from "primeng/datepicker";
import { Dialog } from "primeng/dialog";
import { PaginatorModule, PaginatorState } from "primeng/paginator";
import { ScrollPanelModule } from "primeng/scrollpanel";
import { SkeletonModule } from "primeng/skeleton";
import { SliderModule } from "primeng/slider";
import { ErrorService } from "../../../entities/error";
import { IError } from "../../../entities/error/model/error";
import { MoviesService } from "../../../entities/movies/api/movies.service";
import { Genre, Movie } from "../../../entities/movies/model/movie";
import {
  MoviesPaginationPayload,
  PaginationWrapper,
} from "../../../entities/movies/model/pagination";
import { ToastService, ToastStatus } from "../../../entities/toast";
import { Base64ToImagePipe } from "../../../shared/lib/pipes/base64-to-image.pipe";
import { MinutesToHoursPipe } from "../../../shared/lib/pipes/minutes-to-hours.pipe";
import { ToggleButtonComponent } from "../../../shared/ui/components/toggle-button/toggle-button.component";

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
    ToggleButtonComponent,
    ScrollPanelModule,
    SliderModule,
    FormsModule,
    RouterLink,
  ],
  templateUrl: "./poster.component.html",
  styleUrl: "./poster.component.scss",
})
export class PosterComponent {
  moviesService = inject(MoviesService);
  toastService = inject(ToastService);
  errorService = inject(ErrorService);

  rangeValues: number[] = [20, 80];

  isDatepickerOpen = signal<boolean>(false);
  selectedDate: Date | null = null;
  isFilterDialogVisible: boolean = false;

  readonly DATE_TODAY: string = "Today";
  dateHeading: typeof this.DATE_TODAY | string = this.DATE_TODAY;
  dateHeadingTemp: typeof this.DATE_TODAY | string = this.DATE_TODAY;

  dateNow: Date = new Date();

  total = signal<number>(0);

  movies: Signal<PaginationWrapper<Movie> | null> = this.moviesService.movies;
  payload = signal<MoviesPaginationPayload>({
    limit: 3,
    offset: 1,
    filters: [],
    filterValues: [],
    sortBy: "title",
    sortDirection: "asc",
    date: format(new Date(), "dd-MM-yyyy"),
  });
  isMoviesLoaded: boolean = false;

  genres: Signal<Genre[] | null> = this.moviesService.genres;
  selectedGenres = signal<string[]>([]);

  constructor() {
    effect(() => {
      if (!this.isMoviesLoaded) {
        this.fetchMovies();
        this.isMoviesLoaded = true;
      }
    });
  }

  first: number = 0;

  fetchMovies() {
    this.moviesService.movies.set(null);

    this.moviesService.get(this.payload()).subscribe({
      next: (res: PaginationWrapper<Movie>) => {
        this.total.set(res.total);
      },
      error: (error: IError) => {
        const errorMessage = this.errorService.getErrorMessage(error);
        this.toastService.showToast(ToastStatus.Error, errorMessage);
      },
    });

    this.moviesService.getGenres().subscribe({
      next: (res: Genre[]) => {},
    });
  }

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

  removeFilter(name: string) {
    this.payload.update((val) => {
      const indexToRemove = val.filterValues.indexOf(name);

      if (indexToRemove > -1) {
        const newFilters = [...val.filters];
        const newFilterValues = [...val.filterValues];

        newFilters.splice(indexToRemove, 1);
        newFilterValues.splice(indexToRemove, 1);

        return {
          ...val,
          filters: newFilters,
          filterValues: newFilterValues,
        };
      }
      return val;
    });

    this.selectedGenres.update((genres) => genres.filter((g) => g !== name));

    this.fetchMovies();
  }

  saveFilters() {
    this.isFilterDialogVisible = false;

    this.dateHeading = this.dateHeadingTemp;

    this.payload.update((val) => ({
      ...val,
      filters: this.selectedGenres().map((val) => "genre"),
      filterValues: this.selectedGenres().map((val) => val),
    }));

    this.fetchMovies();
  }

  openDatepicker() {
    this.isDatepickerOpen.set(!this.isDatepickerOpen());
  }

  setDateToday() {
    this.dateHeading = this.DATE_TODAY;

    this.selectedDate = this.dateNow;

    this.payload.update((val) => ({
      ...val,
      date: format(this.dateNow, "dd-MM-yyyy"),
    }));

    if (!this.isMoviesLoaded) {
      this.fetchMovies();
      this.isMoviesLoaded = true;
    }
  }

  onDatepickerChange(date: Date) {
    if (date && date.getDate() !== this.dateNow.getDate()) {
      this.payload.update((val) => ({
        ...val,
        date: format(date, "dd-MM-yyyy"),
      }));

      this.fetchMovies();

      const formatter = new Intl.DateTimeFormat("en-US", {
        month: "long",
        day: "numeric",
      });

      const formattedDate = formatter.format(date).replace(" ", ", ");
      this.dateHeading = formattedDate;
    } else {
      this.dateHeading = "Today";
      this.payload.update((val) => ({
        ...val,
        date: format(this.dateNow, "dd-MM-yyyy"),
      }));

      this.fetchMovies();
    }

    this.isDatepickerOpen.set(false);
  }

  onDatepickerChangeInDialog(date: Date) {
    if (date && date.getDate() !== this.dateNow.getDate()) {
      this.payload.update((val) => ({
        ...val,
        date: format(date, "dd-MM-yyyy"),
      }));

      const formatter = new Intl.DateTimeFormat("en-US", {
        month: "long",
        day: "numeric",
      });

      const formattedDate = formatter.format(date).replace(" ", ", ");
      this.dateHeadingTemp = formattedDate;
    } else {
      this.dateHeadingTemp = "Today";

      this.payload.update((val) => ({
        ...val,
        date: format(this.dateNow, "dd-MM-yyyy"),
      }));
    }

    this.isDatepickerOpen.set(false);
  }

  onGenreToggle(genre: string): void {
    if (this.selectedGenres().some((g: string) => g === genre)) {
      this.selectedGenres.update((genres) =>
        genres.filter((g: string) => g !== genre),
      );
    } else {
      this.selectedGenres.update((genres) => [...genres, genre]);
    }
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
