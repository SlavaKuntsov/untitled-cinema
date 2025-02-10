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
import { ErrorService } from "../../../../app/core/services/error/error.service";
import { ToastService, ToastStatus } from "../../../../app/core/services/toast";
import { IError } from "../../../../entities/error/model/error";
import { Login } from "../../../../entities/users";
import { AuthService } from "../../api/auth.service";
import { passwordValidator } from "../../model/passwordValidation";

@Component({
  selector: "app-login",
  imports: [ReactiveFormsModule, ToastModule, ButtonModule, RouterLink],
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
          this.toastService.showToast(ToastStatus.Success, "Login Success");
          this.router.navigate(["/"]);
          this.authService.accessToken = res;
        },
        error: (error: IError) => {
          const errorMessage = this.errorService.getErrorMessage(error);
          this.toastService.showToast(ToastStatus.Error, errorMessage);
        },
      });

      this.authService.authorize().subscribe();
      return;
    }

    this.toastService.showToast(ToastStatus.Error, "Invalid form data");
  }
}
