import { inject } from "@angular/core";
import { toObservable } from "@angular/core/rxjs-interop";
import { Router } from "@angular/router";
import { switchMap } from "rxjs/operators";
import { UserService } from "../../../../entities/users/api/user.service";

export const canActivateAuth = () => {
  const userService = inject(UserService);
  const router = inject(Router);

  const user$ = toObservable(userService.user);

  console.log("QQQQQQQQ");
  console.log(userService.user());

  return user$.pipe(
    switchMap((user) => {
      console.log("GUARD");
      console.log(user);
      if (user) {
        return [true];
      } else {
        router.navigate(["/auth"]);
        return [false];
      }
    }),
  );
};
