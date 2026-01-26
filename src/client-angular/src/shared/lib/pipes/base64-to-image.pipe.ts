import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "base64ToImage",
})
export class Base64ToImagePipe implements PipeTransform {
  transform(base64: string): string {
    if (!base64) {
      return "";
    }
    return `data:image/jpeg;base64,${base64}`;
  }
}
