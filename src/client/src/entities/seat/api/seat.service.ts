import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { SeatType } from "../model/seat";

@Injectable({
  providedIn: "root",
})
export class SeatService {
  private http = inject(HttpClient);

  get(payload: string): Observable<SeatType[]> {
    return this.http.get<SeatType[]>(
      `${environment.movieBaseUrl}/seats/types/hall/${payload}`,
    );
  }
}
