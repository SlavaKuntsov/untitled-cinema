import { HttpClient } from "@angular/common/http";
import { inject, Injectable, signal } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, map, Observable, tap, throwError } from "rxjs";
import { Login, Registration, User } from "..";
import { userBaseUrl } from "../../../shared/config/backend";
import { UserService } from "./user.service";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private userService = inject(UserService);

  // private isUserObject = new BehaviorSubject<boolean>(this.isAuth);
  // isUser$ = this.isUserObject.asObservable();

  accessTokenExist = signal<boolean>(this.isAuth);

  accessToken = signal<string | null>(localStorage.getItem("yummy-apple"));

  get isAuth(): boolean {
    return !!localStorage.getItem("yummy-apple");
  }

  // private updateUserState() {
  //   this.isUserObject.next(this.userService.isUser);
  // }

  login(payload: Login): Observable<string> {
    return this.http
      .post<{ accessToken: string }>(`${userBaseUrl}/users/login`, payload, {
        withCredentials: true,
        responseType: "json",
      })
      .pipe(
        map((res) => {
          return res.accessToken;
        }),
        tap((res) => {
          this.saveTokens(res);
        }),
        // catchError((error: HttpErrorResponse) => {
        //   console.log("EEEEEEEE");
        //   console.log(error);
        //   // Обрабатываем ошибку
        //   if (error.error && error.error.detail) {
        //     // Если есть поле detail, возвращаем его
        //     return throwError(() => error.error.detail);
        //   }
        //   // Иначе возвращаем общее сообщение об ошибке
        //   return throwError(() => "An unknown error occurred");
        // }),
      );
  }

  registration(payload: Registration): Observable<string> {
    return this.http
      .post<{ accessToken: string }>(
        `${userBaseUrl}/users/registration`,
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
          // this.authorize().subscribe();
        }),
      );
  }

  authorize(): Observable<User> {
    return this.http.get<User>(`${userBaseUrl}/auth/authorize`).pipe(
      tap((res) => {
        this.userService.user.set(res);
        console.log("AUTH");
      }),
    );
  }

  refreshToken(): Observable<string> {
    return this.http
      .get<{ accessToken: string }>(`${userBaseUrl}/auth/refreshToken`, {
        responseType: "json",
      })
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
    return this.http.get(`${userBaseUrl}/auth/unauthorize`).pipe(
      tap(() => {
        console.log("LOGOOOOOOUT");
        localStorage.removeItem("yummy-apple");
        this.accessToken.set(null);
        this.userService.user.set(null);
        // this.updateUserState();
        this.router.navigate(["/auth"]);
      }),
    );
  }
	
  private saveTokens(res: string) {
    console.log("saveTokens");
    console.log(res);

    this.accessToken.set(res);
    localStorage.setItem("yummy-apple", res);

    console.log(1);
    console.log(localStorage.getItem("yummy-apple"));
  }
}
