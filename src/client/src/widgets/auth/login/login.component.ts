import { Component, inject, signal } from "@angular/core";
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { ButtonModule } from "primeng/button";
import { ToastModule } from "primeng/toast";
import { ErrorService } from "../../../entities/error";
import { IError } from "../../../entities/error/model/error";
import { ToastService, ToastStatus } from "../../../entities/toast";
import { Login, User } from "../../../entities/users";
import { AuthService } from "../../../entities/users/api/auth.service";
import { passwordValidator } from "../../../shared/lib/model/password-validation";
import { ButtonComponent } from "../../../shared/ui/components/button/button.component";
import { InputComponent } from "../../../shared/ui/components/input/input.component";

@Component({
  selector: "app-login",
  imports: [
    ReactiveFormsModule,
    ToastModule,
    ButtonModule,
    RouterLink,
    InputComponent,
    ButtonComponent,
  ],
  templateUrl: "./login.component.html",
  styleUrl: "./login.component.scss",
})
export class LoginComponent {
  toastService = inject(ToastService);
  authService = inject(AuthService);
  errorService = inject(ErrorService);
  router = inject(Router);

  form = new FormGroup({
    email: new FormControl<string | null>("example@email.com", [
      Validators.required,
      Validators.email,
    ]),
    password: new FormControl<string | null>("qweQWE123", [
      Validators.required,
      Validators.minLength(5),
      passwordValidator(),
    ]),
  });

  get email() {
    return this.form.get("email");
  }

  get password() {
    return this.form.get("password");
  }

  isPasswordVisible = signal<boolean>(false);

  onSubmit() {
    if (this.form.valid) {
      const loginData: Login = {
        email: this.email?.value ?? "",
        password: this.password?.value ?? "",
      };

      this.authService.login(loginData).subscribe({
        next: (res: string) => {
          localStorage.setItem("yummy-apple", res);
          this.toastService.showToast(ToastStatus.Success, "Login Success");
          this.authService.accessToken.set(res);
        },
        error: (error: IError) => {
          const errorMessage = this.errorService.getErrorMessage(error);
          this.toastService.showToast(ToastStatus.Error, errorMessage);
        },
        complete: () => {
          this.authService.authorize().subscribe({
            next: (res: User) => {
            },
          });

          this.router.navigate(["/"]);
        },
      });

      return;
    }

    this.toastService.showToast(ToastStatus.Error, "Invalid form data");
  }
}
