import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, catchError, Observable, tap, throwError } from "rxjs";
import { Login, Registration, User } from "../../../entities/users";
import { userBaseUrl } from "../../../shared/config/backend";
import { ToastService } from "./../../../app/core/services/toast/model/toast.service";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private toastService = inject(ToastService);

  private isUserObject = new BehaviorSubject<boolean>(this.isAuth);
  isUser$ = this.isUserObject.asObservable();

  accessToken: string | null = localStorage.getItem("yummy-apple");
  user: User | null = null;

  get isAuth(): boolean {
    return !!localStorage.getItem("yummy-apple");
  }

  get isUser(): boolean {
    return !!this.user;
  }

  private updateUserState() {
    this.isUserObject.next(this.isUser);
  }

  login(payload: Login): Observable<string> {
    return this.http
      .post(`${userBaseUrl}/Users/Login`, payload, {
        withCredentials: true,
        responseType: "text",
      })
      .pipe(
        tap((res) => {
          this.saveTokens(res);
          this.authorize().subscribe();
        }),
      );
  }

  registration(payload: Registration): Observable<string> {
    return this.http
      .post(`${userBaseUrl}/Users/Registration`, payload, {
        withCredentials: true,
        responseType: "text",
      })
      .pipe(
        tap((res) => {
          this.saveTokens(res);
          this.authorize().subscribe();
        }),
      );
  }

  authorize() {
    return this.http.get<User>(`${userBaseUrl}/Auth/Authorize`).pipe(
      tap((res) => {
        this.user = res;
        this.updateUserState();
      }),
    );
  }

  refreshToken() {
    return this.http
      .get(`${userBaseUrl}/Auth/RefreshToken`, {
        responseType: "text",
      })
      .pipe(
        tap((res) => {
          this.saveTokens(res);
        }),
        catchError((error) => {
          return throwError(() => error);
        }),
      );
  }

  logout() {
    return this.http.get(`${userBaseUrl}/Auth/Unauthorize`).pipe(
      tap(() => {
        localStorage.removeItem("yummy-apple");
        this.accessToken = null;
        this.user = null;
        this.updateUserState();
        this.router.navigate(["/auth"]);
      }),
    );
  }

  private saveTokens(res: string) {
    this.accessToken = res;
    localStorage.setItem("yummy-apple", res);
  }
}
