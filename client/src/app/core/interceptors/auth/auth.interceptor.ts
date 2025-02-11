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

  // Если токен есть, добавляем его в запрос
  if (accessToken) {
    return next(addToken(accessToken, req)).pipe(
      catchError((error) => {
        // Если ошибка 401, пробуем обновить токен
        if (error.status === 401) {
          return handle401Error(authService, req, next);
        }
        // Если другая ошибка, просто пробрасываем её
        return throwError(() => error);
      }),
    );
  }

  // Если токена нет, просто пропускаем запрос
  return next(req);
};

const handle401Error = (
  authService: AuthService,
  req: HttpRequest<any>,
  next: HttpHandlerFn,
) => {
  return authService.refreshToken().pipe(
    switchMap((newToken) => {
      // Если refreshToken успешен, добавляем новый токен в запрос
      return next(addToken(newToken, req));
    }),
    catchError((error) => {
      // Если refreshToken завершился ошибкой, перенаправляем на страницу входа
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
