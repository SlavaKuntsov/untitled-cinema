import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { NavComponent } from "./ui/nav/nav.component";

@Component({
  selector: "app-home",
  imports: [RouterOutlet, NavComponent],
  templateUrl: "./home.component.html",
  styleUrl: "./home.component.scss",
})
export class HomeComponent {}
