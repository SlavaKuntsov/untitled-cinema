import { Component, inject, signal } from "@angular/core";
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { format, parse } from "date-fns";
import { ButtonModule } from "primeng/button";
import { DatePickerModule } from "primeng/datepicker";
import { ToastModule } from "primeng/toast";
import { ErrorService, IError } from "../../../entities/error";
import { ToastService, ToastStatus } from "../../../entities/toast";
import { Registration, User } from "../../../entities/users";
import { AuthService } from "../../../entities/users/api/auth.service";
import { passwordValidator } from "../../../shared/lib/model/password-validation";
import { ButtonComponent } from "../../../shared/ui/components/button/button.component";
import { InputComponent } from "../../../shared/ui/components/input/input.component";

@Component({
  selector: "app-registration",
  imports: [
    ReactiveFormsModule,
    ToastModule,
    ButtonModule,
    RouterLink,
    DatePickerModule,
    InputComponent,
    ButtonComponent,
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
    dateOfBirth: new FormControl<Date | null>(
      parse("20-12-2005", "dd-MM-yyyy", new Date()),
      [Validators.required],
    ),
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

  get dateOfBirth() {
    return this.form.get("dateOfBirth");
  }

  isPasswordVisible = signal<boolean>(false);

  onSubmit() {
    if (this.form.valid) {
      const loginData: Registration = {
        email: this.email?.value ?? "",
        password: this.password?.value ?? "",
        firstName: this.firstName?.value ?? "",
        lastName: this.lastName?.value ?? "",
        dateOfBirth: this.dateOfBirth?.value
          ? format(this.dateOfBirth.value, "dd-MM-yyyy")
          : "",
      };

      this.authService.registration(loginData).subscribe({
        next: (res: string) => {
          this.toastService.showToast(
            ToastStatus.Success,
            "Registration Success",
          );
          localStorage.setItem("yummy-apple", res);
          this.router.navigate(["/"]);
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
        },
      });

      this.authService.authorize().subscribe();
      return;
    }

    this.toastService.showToast(ToastStatus.Error, "Invalid form data");
  }
}
