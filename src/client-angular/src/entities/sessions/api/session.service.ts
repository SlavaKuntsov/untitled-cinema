import { HttpClient, HttpParams } from "@angular/common/http";
import { effect, inject, Injectable, signal } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { SessionsPaginationPayload } from "../model/pagination";
import { Session } from "../model/session";

@Injectable({
  providedIn: "root",
})
export class SessionService {
  private http = inject(HttpClient);

  selectedSessionId = signal<string | null>(localStorage.getItem("sessionId"));

  constructor() {
    effect(() => {
      localStorage.setItem("sessionId", this.selectedSessionId()!);
    });
  }

  get(payload: SessionsPaginationPayload): Observable<Session[]> {
    let params = new HttpParams()
      .set("Limit", payload.limit.toString())
      .set("Offset", payload.offset.toString())
      .set("Movie", payload.movie.toString())
      .set("Date", payload.date)
      .set("Hall", payload.hall);

    return this.http.get<Session[]>(`${environment.movieBaseUrl}/sessions`, {
      params,
    });
  }

  getSession(payload: string): Observable<Session> {
    return this.http.get<Session>(
      `${environment.movieBaseUrl}/sessions/${payload}`,
    );
  }
}
