import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";

@Component({
  selector: "app-button",
  imports: [CommonModule], // Добавляем CommonModule для ngClass
  templateUrl: "./button.component.html",
  styleUrl: "./button.component.scss",
})
export class ButtonComponent {
  @Input() label: string = ""; // Текст кнопки
  @Input() buttonClasses: string | string[] = ""; // Классы для кнопки
  @Output() clickEvent = new EventEmitter<void>(); // Событие клика

  onClick() {
    this.clickEvent.emit(); // Генерация события при клике
  }
}
