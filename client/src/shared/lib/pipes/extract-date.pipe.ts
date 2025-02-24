import { Pipe, PipeTransform } from "@angular/core";
import { format, isValid, parse } from "date-fns";

@Pipe({
  name: "extractDate",
})
export class ExtractDatePipe implements PipeTransform {
  transform(value: string): string {
    if (!value) return "";

    try {
      const date = parse(value, "dd-MM-yyyy HH:mm", new Date());

      if (!isValid(date)) return "Invalid Date";

      return format(date, "dd.MM");
    } catch (e) {
      return "Invalid Format";
    }
  }
}
