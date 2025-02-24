import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "extractTime",
})
export class ExtractTimePipe implements PipeTransform {
  transform(value: string): string {
    if (!value) {
      return "";
    }

    const parts = value.split(" ");
    if (parts.length > 1) {
      return parts[1];
    }

    return value;
  }
}
