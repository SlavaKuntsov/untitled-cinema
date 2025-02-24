import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";

@Component({
  selector: "app-toggle-button",
  imports: [CommonModule],
  templateUrl: "./toggle-button.component.html",
  styleUrl: "./toggle-button.component.scss",
})
export class ToggleButtonComponent {
  @Input() genre!: string;
  @Input() selected: boolean = false;

  toggleSelection(): void {
    this.selected = !this.selected;
  }
}
