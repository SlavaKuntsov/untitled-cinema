import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { NavComponent } from "./ui/nav/nav.component";

@Component({
  selector: "app-home-page",
  imports: [RouterOutlet, NavComponent],
  templateUrl: "./home-page.component.html",
  styleUrl: "./home-page.component.scss",
})
export class HomePageComponent {}
