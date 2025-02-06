import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";

@Component({
  selector: "app-nav",
  imports: [RouterLink],
  templateUrl: "./nav.component.html",
  styleUrl: "./nav.component.scss",
})
export class NavComponent {
  routes = [
    { route: "/", title: "Home" },
    { route: "/poster", title: "Poster" },
  ];
}
