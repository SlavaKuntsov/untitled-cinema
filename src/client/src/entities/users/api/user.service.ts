import { HttpClient } from "@angular/common/http";
import { effect, inject, Injectable, signal } from "@angular/core";
import { Observable, tap } from "rxjs";
import { UpdateUser, User } from "..";
import { environment } from "../../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class UserService {
  private http = inject(HttpClient);

  user = signal<User | null>(null);

  constructor() {
    effect(() => {});
  }

  update(payload: UpdateUser): Observable<User> {
    return this.http
      .patch<User>(`${environment.userBaseUrl}/users`, payload, {
        withCredentials: true,
      })
      .pipe(
        tap((res) => {
          this.user.set(res);
        }),
      );
  }

  delete() {
    return this.http
      .delete(`${environment.userBaseUrl}/users/me`, {
        withCredentials: true,
      })
      .pipe(
        tap(() => {
          this.user.set(null);
        }),
      );
  }
}
