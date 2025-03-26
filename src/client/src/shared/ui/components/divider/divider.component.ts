import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";

@Component({
  selector: "app-divider",
  imports: [CommonModule],
  template: `<span
    class="block h-1 w-full min-w-full rounded-md"
    [ngClass]="dividerClasses"
  ></span>`,
  styles: `
    :host {
      @apply block;
    }
  `,
})
export class DividerComponent {
  @Input() dividerClasses: string | string[] = "bg-zinc-800 my-6";
}
