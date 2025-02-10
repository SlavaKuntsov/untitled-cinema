import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { ToastModule } from "primeng/toast";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, ToastModule],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  authService = inject(AuthService);
  ngOnInit() {
    this.authService.authorize().subscribe((val) => console.log(val));
  }
