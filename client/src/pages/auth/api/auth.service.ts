import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { CookieService } from "ngx-cookie-service";
import { BehaviorSubject, catchError, Observable, tap, throwError } from "rxjs";
import { Login, User } from "../../../entities/users";
import { userBaseUrl } from "../../../shared/config/backend";
import { ToastService } from "./../../../app/core/services/toast/model/toast.service";
import { ToastStatus } from "../../../app/core/services/toast";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private cookieService = inject(CookieService);
  private toastService = inject(ToastService);

  private isUserObject = new BehaviorSubject<boolean>(this.isUser);
  isUser$ = this.isUserObject.asObservable();

  accessToken: string | null = null;
  user: User | null = null;

  get isAuth(): boolean {
    return !!localStorage.getItem("yummy-apple");
  }

  get isUser(): boolean {
    return !!this.user;
  }

  login(payload: Login): Observable<string> {
    return this.http
      .post(`${userBaseUrl}/Users/Login`, payload, {
        withCredentials: true,
        responseType: "text",
      })
      .pipe(
        tap((res) => {
          console.log("after login");
          this.saveTokens(res);
          // this.authorize();
        }),
      );
  }

  authorize() {
    return this.http.get<User>(`${userBaseUrl}/Auth/Authorize`).pipe(
      tap((res) => {
        console.log("after authorize");
        this.user = res;
        this.isUserObject.next(this.isUser);

        this.toastService.showToast(ToastStatus.Success, res.email);
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
          console.log(error);
          return throwError(() => error);
        }),
      );
  }

  logout() {
    return this.http.get(`${userBaseUrl}/Auth/Unauthorize`).pipe(
      tap((res) => {
        localStorage.removeItem("yummy-apple");
        this.accessToken = null;

        this.user = null;
        this.isUserObject.next(false);

        this.router.navigate(["/auth"]);
      }),
    );
  }

  saveTokens(res: string) {
    this.accessToken = res;
    localStorage.setItem("yummy-apple", res);
  }
}
