import { HttpClient, HttpParams } from "@angular/common/http";
import { effect, inject, Injectable, signal } from "@angular/core";
import { Observable, tap } from "rxjs";
import { environment } from "../../../environments/environment";
import { Genre, Movie } from "../model/movie";
import {
  MoviesPaginationPayload,
  PaginationWrapper,
} from "../model/pagination";

@Injectable({
  providedIn: "root",
})
export class MoviesService {
  private http = inject(HttpClient);

  movies = signal<PaginationWrapper<Movie> | null>(null);
  genres = signal<Genre[] | null>(null);

  selectedMovieId = signal<string | null>(localStorage.getItem("movieId"));

  constructor() {
    effect(() => {
      localStorage.setItem("movieId", this.selectedMovieId()!);
    });
  }

  get(payload: MoviesPaginationPayload): Observable<PaginationWrapper<Movie>> {
    let params = new HttpParams()
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

    return this.http
      .get<PaginationWrapper<Movie>>(`${environment.movieBaseUrl}/movies`, {
        params,
      })
      .pipe(
        tap((res) => {
          this.movies.set(res);
        }),
      );
  }

  getMovie(payload: string): Observable<Movie> {
    return this.http.get<Movie>(
      `${environment.movieBaseUrl}/movies/${payload}`,
    );
  }

  getGenres(): Observable<Genre[]> {
    return this.http
      .get<Genre[]>(`${environment.movieBaseUrl}/movies/genres`)
      .pipe(
        tap((res) => {
          this.genres.set(res);
        }),
      );
  }
}
