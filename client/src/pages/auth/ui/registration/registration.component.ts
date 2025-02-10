import { Component, inject, signal } from "@angular/core";
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { ButtonModule } from "primeng/button";
import { DatePickerModule } from "primeng/datepicker";
import { ToastModule } from "primeng/toast";
import { ErrorService } from "../../../../app/core/services/error/error.service";
import { ToastService, ToastStatus } from "../../../../app/core/services/toast";
import { IError } from "../../../../entities/error/model/error";
import { Registration } from "../../../../entities/users";
import { AuthService } from "../../api/auth.service";
import { passwordValidator } from "../../model/passwordValidation";

@Component({
  selector: "app-registration",
  imports: [
    ReactiveFormsModule,
    ToastModule,
    ButtonModule,
    RouterLink,
    DatePickerModule,
  ],
  templateUrl: "./registration.component.html",
  styleUrl: "./registration.component.scss",
})
export class RegistrationComponent {
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
    firstName: new FormControl<string | null>("Slava", [Validators.required]),
    lastName: new FormControl<string | null>("Kuntsov", [Validators.required]),
    dateOfBirth: new FormControl<Date | null>(null, [Validators.required]),
  });

  get email() {
    return this.form.get("email");
  }

  get password() {
    return this.form.get("password");
  }

  get firstName() {
    return this.form.get("firstName");
  }

  get lastName() {
    return this.form.get("lastName");
  }

  isPasswordVisible = signal<boolean>(false);

  onSubmit() {
    if (this.form.valid) {
      const loginData: Registration = {
        email: this.email?.value ?? "",
        password: this.password?.value ?? "",
        firstName: this.firstName?.value ?? "",
        lastName: this.lastName?.value ?? "",
        dateOfBirth: "28-05-2005",
      };

      this.authService.registration(loginData).subscribe({
        next: (res: string) => {
          this.toastService.showToast(
            ToastStatus.Success,
            "Registration Success",
          );
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
