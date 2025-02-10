import {
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, switchMap, throwError } from "rxjs";
import { AuthService } from "../../../../pages/auth/api/auth.service";

let isRefreshing: boolean = false;

export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  console.log(authService.accessToken);
  console.log(1);

  if (authService.accessToken) {
    // if (!authService.accessToken || authService.accessToken == null) {
    if (!isRefreshing) {
      console.log(11);
      console.log(authService.accessToken);
      return refreshToken(authService, req, next);
    }

    console.log(2);

    if (isRefreshing) {
      return refreshToken(authService, req, next);
    }
  }

  console.log(3);

  return next(addToken(authService, authService.accessToken!, req)).pipe(
    catchError((error) => {
      if (error.status === 401) {
        return refreshToken(authService, req, next);
      }
      return throwError(() => error);
    }),
  );
};

const refreshToken = (
  authService: AuthService,
  req: HttpRequest<any>,
  next: HttpHandlerFn,
) => {
  if (!isRefreshing) {
    isRefreshing = true;

    console.log("REFRESH TOKEN INTERSEPTOR");

    return authService.refreshToken().pipe(
      switchMap((res) => {
        isRefreshing = false;

        return next(addToken(authService, res, req));
      }),
    );
  }

  console.log("ACCESS TOKEN INTERSEPTOR");

  return next(addToken(authService, authService.accessToken!, req));
};

const addToken = (
  authSeervice: AuthService,
  token: string,
  req: HttpRequest<any>,
) => {
  console.log("access token " + authSeervice.accessToken);
  console.log(`Bearer ${token}`);

  return req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
    withCredentials: true,
  });
};
