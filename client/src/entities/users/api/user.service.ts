import { HttpClient } from "@angular/common/http";
import { effect, inject, Injectable, signal } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, tap } from "rxjs";
import { UpdateUser, User } from "..";
import { userBaseUrl } from "../../../shared/config/backend";

@Injectable({
  providedIn: "root",
})
export class UserService {
  private http = inject(HttpClient);
  private router = inject(Router);

  // userObject = new BehaviorSubject<User | null>(null);
  // user$ = this.userObject.asObservable();

  user = signal<User | null>(null);

  constructor() {
    effect(() => {
      console.log("EFFECT_______________________");
      console.log(this.user());
    });
  }

  // get isUser(): boolean {
  //   return !!this.userObject;
  // }

  update(payload: UpdateUser): Observable<User> {
    return this.http
      .patch<User>(`${userBaseUrl}/users`, payload, {
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
      .delete(`${userBaseUrl}/users/me`, {
        withCredentials: true,
      })
      .pipe(
        tap(() => {
          this.user.set(null);
        }),
      );
  }
}
