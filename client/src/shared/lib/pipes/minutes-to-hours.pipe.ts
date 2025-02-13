import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "minutesToHours",
})
export class MinutesToHoursPipe implements PipeTransform {
  transform(minutes: number): string {
    if (isNaN(minutes)) {
      return ""; // Возвращаем пустую строку, если значение не является числом
    }

    const hours = Math.floor(minutes / 60); // Получаем количество часов
    const remainingMinutes = minutes % 60; // Получаем оставшиеся минуты

    // Форматируем результат
    if (hours === 0) {
      return `${remainingMinutes} мин`; // Только минуты, если часов нет
    } else if (remainingMinutes === 0) {
      return `${hours} ч`; // Только часы, если минут нет
    } else {
      return `${hours} ч ${remainingMinutes} мин`; // Часы и минуты
    }
  }
}
