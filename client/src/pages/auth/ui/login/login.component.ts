import { Component, inject, signal } from "@angular/core";
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { ButtonModule } from "primeng/button";
import { ToastModule } from "primeng/toast";
import { ToastService, ToastStatus } from "../../../../app/core/services/toast";
import { IError } from "../../../../entities/error/model/error";
import { Login } from "../../../../entities/users";
import { AuthService } from "../../api/auth.service";

@Component({
  selector: "app-login",
  imports: [ReactiveFormsModule, ToastModule, ButtonModule, RouterLink],
  templateUrl: "./login.component.html",
  styleUrl: "./login.component.scss",
})
export class LoginComponent {
  toastService = inject(ToastService);
  authService = inject(AuthService);
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
          this.toastService.showToast(ToastStatus.Success, "Login Success");
          this.router.navigate(["/"]);
          this.authService.accessToken = res;
          console.log(res);
        },
        error: (error: IError) => {
          this.toastService.showToast(ToastStatus.Error, error.error.detail);
        },
      });
      return;
    }

    this.toastService.showToast(
      ToastStatus.Error,
      "Please write Email and Password",
    );
  }
}

export function passwordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    if (value === "admin") {
      return null; // ✅ Разрешаем "admin" без проверки
    }

    // Проверяем пароль на сложность
    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumber = /\d/.test(value);
    const isValid = hasUpperCase && hasLowerCase && hasNumber;

    return isValid
      ? null
      : {
          passwordInvalid:
            "Пароль должен содержать цифры, строчные и заглавные буквы",
        };
  };
}
