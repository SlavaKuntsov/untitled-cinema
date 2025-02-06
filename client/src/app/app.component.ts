import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { NavComponent } from "../shared/ui/components/nav/nav.component";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, NavComponent],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {}
