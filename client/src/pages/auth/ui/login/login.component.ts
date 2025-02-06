import { Component, inject, signal } from "@angular/core";
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { RouterLink } from "@angular/router";
import { MessageService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { ToastModule } from "primeng/toast";

@Component({
  selector: "app-login",
  imports: [ReactiveFormsModule, ToastModule, ButtonModule, RouterLink],
  templateUrl: "./login.component.html",
  styleUrl: "./login.component.scss",
})
export class LoginComponent {
  messageService = inject(MessageService);
  showSuccess() {
    this.messageService.add({
      severity: "success",
      summary: "Success",
      detail: "Login successful",
    });
  }

  form = new FormGroup({
    email: new FormControl<string | null>(null, Validators.required),
    password: new FormControl<string | null>(null, Validators.required),
  });

  isPasswordVisible = signal<boolean>(false);

  onSubmit() {
    if (this.form.valid) {
      console.log(this.form.value);
      this.messageService.add({
        severity: "success",
        summary: "Success",
        detail: "Login successful",
      });
      return;
    }
    this.messageService.add({
      severity: "error",
      summary: "Success",
      detail: "Login successful",
    });
  }
}
