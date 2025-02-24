import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { Hall } from "..";
import { environment } from "../../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class HallService {
  private http = inject(HttpClient);

  getHall(payload: string): Observable<Hall> {
    return this.http.get<Hall>(`${environment.movieBaseUrl}/halls/${payload}`);
  }
}
