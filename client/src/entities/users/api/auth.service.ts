import { HttpClient } from "@angular/common/http";
import { inject, Injectable, signal } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, map, Observable, tap, throwError } from "rxjs";
import { Login, Registration, User } from "..";
import { environment } from "../../../environments/environment";
import { UserService } from "./user.service";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private userService = inject(UserService);

  accessTokenExist = signal<boolean>(this.isAuth);

  accessToken = signal<string | null>(localStorage.getItem("yummy-apple"));

  get isAuth(): boolean {
    return !!localStorage.getItem("yummy-apple");
  }

  login(payload: Login): Observable<string> {
    return this.http
      .post<{ accessToken: string }>(
        `${environment.userBaseUrl}/users/login`,
        payload,
        {
          withCredentials: true,
          responseType: "json",
        },
      )
      .pipe(
        map((res) => {
          return res.accessToken;
        }),
        tap((res) => {
          this.saveTokens(res);
        }),
      );
  }

  registration(payload: Registration): Observable<string> {
    return this.http
      .post<{ accessToken: string }>(
        `${environment.userBaseUrl}/users/registration`,
        payload,
        {
          withCredentials: true,
          responseType: "json",
        },
      )
      .pipe(
        map((res) => {
          return res.accessToken;
        }),
        tap((res) => {
          this.saveTokens(res);
        }),
      );
  }

  authorize(): Observable<User> {
    return this.http
      .get<User>(`${environment.userBaseUrl}/auth/authorize`)
      .pipe(
        tap((res) => {
          this.userService.user.set(res);
        }),
      );
  }

  refreshToken(): Observable<string> {
    return this.http
      .get<{ accessToken: string }>(
        `${environment.userBaseUrl}/auth/refreshToken`,
        {
          responseType: "json",
        },
      )
      .pipe(
        map((res) => res.accessToken),
        tap((res) => {
          this.saveTokens(res);
        }),
        catchError((error) => {
          this.logout();
          return throwError(() => error);
        }),
      );
  }

  logout() {
    return this.http.get(`${environment.userBaseUrl}/auth/unauthorize`).pipe(
      tap(() => {
        localStorage.removeItem("yummy-apple");
        this.accessToken.set(null);
        this.userService.user.set(null);
        this.router.navigate(["/auth"]);
      }),
    );
  }

  private saveTokens(res: string) {

    this.accessToken.set(res);
    localStorage.setItem("yummy-apple", res);
  }
}
