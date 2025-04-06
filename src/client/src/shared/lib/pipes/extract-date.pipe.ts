import { Pipe, PipeTransform } from "@angular/core";
import { format, parseISO } from "date-fns";

@Pipe({
  name: "extractDate",
})
export class ExtractDatePipe implements PipeTransform {
  transform(value: string): string {
    if (!value) return "";

    try {
      const date = parseISO(value); // Используем parseISO для ISO формата
      return format(date, "dd.MM");
    } catch (e) {
      console.error("Date parsing error:", e);
      return "Invalid Date";
    }
  }
}
