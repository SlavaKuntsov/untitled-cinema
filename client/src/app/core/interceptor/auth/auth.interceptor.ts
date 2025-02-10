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

  if (authService.accessToken) {
    if (!isRefreshing) {
      return refreshToken(authService, req, next);
    }

    if (isRefreshing) {
      return refreshToken(authService, req, next);
    }
  }

  return next(addToken(authService.accessToken!, req)).pipe(
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

    return authService.refreshToken().pipe(
      switchMap((res) => {
        isRefreshing = false;

        return next(addToken(res, req));
      }),
    );
  }

  return next(addToken(authService.accessToken!, req));
};

const addToken = (token: string, req: HttpRequest<any>) => {
  return req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
    withCredentials: true,
  });
};
