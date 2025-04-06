import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "extractTime",
})
export class ExtractTimePipe implements PipeTransform {
  transform(value: string): string {
    if (!value) return "";

    try {
      const date = new Date(value);
      return date.toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
      });
    } catch {
      return value; // возвращаем как есть, если не удалось распарсить
    }
  }
}
