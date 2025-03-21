import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "transformDate",
})
export class TransformDatePipe implements PipeTransform {
  transform(value: string): string {
    if (!value) {
      return "";
    }

    const parts = value.split("-");
    if (parts.length !== 3) {
      return value;
    }

    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10);
    const year = parseInt(parts[2], 10);

    const monthNames = [
      "January",
      "February",
      "March",
      "April",
      "May",
      "June",
      "July",
      "August",
      "September",
      "October",
      "November",
      "December",
    ];

    if (month < 1 || month > 12) {
      return value;
    }

    return `${monthNames[month - 1]} ${day}`;
  }
}
