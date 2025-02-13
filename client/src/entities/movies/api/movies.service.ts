import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable, signal } from "@angular/core";
import { Observable, tap } from "rxjs";
import { movieBaseUrl } from "../../../shared/config/backend";
import { Movie } from "../model/movie";
import { PaginationPayload, PaginationWrapper } from "../model/pagination";

@Injectable({
  providedIn: "root",
})
export class MoviesService {
  private http = inject(HttpClient);

  movies = signal<PaginationWrapper<Movie> | null>(null);

  get(payload: PaginationPayload): Observable<PaginationWrapper<Movie>> {
    let params = new HttpParams()
      .set("Limit", payload.limit.toString())
      .set("Offset", payload.offset.toString())
      .set("SortBy", payload.sortBy)
      .set("SortDirection", payload.sortDirection);

    if (payload.filter && payload.filterValue) {
      params = params
        .set("Filter", payload.filter)
        .set("FilterValue", payload.filterValue);
    }

    return this.http
      .get<PaginationWrapper<Movie>>(`${movieBaseUrl}/movies`, {
        params,
        withCredentials: true,
      })
      .pipe(
        tap((res) => {
          this.movies.set(res);
        }),
      );
  }
}
