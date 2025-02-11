import { CommonModule } from "@angular/common";
import { Component, effect, inject, Signal } from "@angular/core";
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { format, parse } from "date-fns";
import { ConfirmationService, MessageService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { DatePickerModule } from "primeng/datepicker";
import { DialogModule } from "primeng/dialog";
import { ToastModule } from "primeng/toast";
import { ToggleSwitch } from "primeng/toggleswitch";
import { ErrorService } from "../../../app/core/services/error/api/error.service";
import { ToastService, ToastStatus } from "../../../app/core/services/toast";
import { IError } from "../../../entities/error/model/error";
import { UpdateUser, User } from "../../../entities/users";
import { AuthService } from "../../../entities/users/api/auth.service";
import { UserService } from "../../../entities/users/api/user.service";
import { ButtonComponent } from "../../../shared/ui/components/button/button.component";
import { InputComponent } from "../../../shared/ui/components/input/input.component";

@Component({
  selector: "app-profile",
  imports: [
    ReactiveFormsModule,
    ToastModule,
    ButtonModule,
    InputComponent,
    CommonModule,
    DialogModule,
    ConfirmDialogModule,
    FormsModule,
    ToggleSwitch,
    DatePickerModule,
    ButtonComponent,
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: "./profile.component.html",
  styleUrl: "./profile.component.scss",
})
export class ProfileComponent {
  authService = inject(AuthService);
  userService = inject(UserService);
  errorService = inject(ErrorService);
  confirmationService = inject(ConfirmationService);
  messageService = inject(MessageService);
  toastService = inject(ToastService);

  checked: boolean = false;

  form = new FormGroup({
    email: new FormControl<string | null>({ value: "", disabled: true }, [
      Validators.required,
      Validators.email,
    ]),
    firstName: new FormControl<string | null>({ value: "", disabled: true }, [
      Validators.required,
    ]),
    lastName: new FormControl<string | null>({ value: "", disabled: true }, [
      Validators.required,
    ]),
    dateOfBirth: new FormControl<Date | null>({ value: null, disabled: true }, [
      Validators.required,
    ]),
  });

  user: Signal<User | null> = this.userService.user;

  newUser: User | null = this.user();

  constructor() {
    effect(() => {
      const user = this.user();
      if (user) {
        this.form.patchValue({
          email: user.email,
          firstName: user.firstName,
          lastName: user.lastName,
          dateOfBirth: user.dateOfBirth
            ? parse(user.dateOfBirth, "dd-MM-yyyy", new Date())
            : null,
        });
      }
    });
  }

  save() {
    this.confirmationService.confirm({
      header: "Are you sure?",
      message: "Please confirm to proceed.",
      key: "save",
      accept: () => {
        console.log(this.form.value);
        const updateUser: UpdateUser = {
          id: this.newUser?.id!,
          firstName: this.newUser?.firstName!,
          lastName: this.newUser?.lastName!,
          dateOfBirth: this.newUser?.dateOfBirth!,
        };

        console.log("UPD");
        console.log(updateUser);

        this.userService.update(updateUser).subscribe({
          next: (res: User) => {
            this.toastService.showToast(ToastStatus.Info, "Profile saved");
          },
          error: (error: IError) => {
            console.log(error);
            const errorMessage = this.errorService.getErrorMessage(error);
            this.toastService.showToast(ToastStatus.Error, errorMessage);
          },
        });
      },
      reject: () => {},
    });
  }

  logout() {
    this.confirmationService.confirm({
      header: "Are you sure?",
      message: "Please confirm to proceed.",
      key: "logout",
      accept: () => {
        this.toastService.showToast(ToastStatus.Info, "Logout");
        this.authService.logout().subscribe();
      },
      reject: () => {},
    });
  }

  delete() {
    this.confirmationService.confirm({
      header: "Are you sure?",
      message: "Please confirm to proceed.",
      key: "delete",
      accept: () => {
        this.toastService.showToast(
          ToastStatus.Warn,
          "Delete your prifile is success",
        );
        this.userService.delete().subscribe({
          next: () => {
            this.authService.logout().subscribe();
          },
        });
      },
      reject: () => {},
    });
  }

  onCancel() {
    console.log("User canceled action");
  }

  onFormChange() {
    if (this.newUser) {
      this.newUser.firstName = this.form.controls.firstName.value!;
      this.newUser.lastName = this.form.controls.lastName.value!;
      this.newUser.dateOfBirth = format(
        this.form.controls.dateOfBirth.value!,
        "dd-MM-yyyy",
      );
      console.log(this.newUser);
    }
  }

  toggleInputs() {
    if (this.checked) {
      this.form.enable();
      this.form.controls.email.disable();
    } else {
      this.form.disable();
    }
  }

  onDatepickerChange() {
    if (this.newUser) {
      this.newUser.dateOfBirth = format(
        this.form.controls.dateOfBirth.value!,
        "dd-MM-yyyy",
      );
    }
  }
}
