import {
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, switchMap, throwError } from "rxjs";
import { AuthService } from "../../../../entities/users/api/auth.service";

export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const accessToken = authService.accessToken();

  if (accessToken) {
    return next(addToken(accessToken, req)).pipe(
      catchError((error) => {
        if (error.status === 401) {
          return handle401Error(authService, req, next);
        }
        return throwError(() => error);
      }),
    );
  }

  return next(req);
};

const handle401Error = (
  authService: AuthService,
  req: HttpRequest<any>,
  next: HttpHandlerFn,
) => {
  return authService.refreshToken().pipe(
    switchMap((newToken) => {
      return next(addToken(newToken, req));
    }),
    catchError((error) => {
      authService.logout();
      return throwError(() => error);
    }),
  );
};

const addToken = (token: string, req: HttpRequest<any>) => {
  return req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
    withCredentials: true,
  });
};
