import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "minutesToHours",
})
export class MinutesToHoursPipe implements PipeTransform {
  transform(minutes: number): string {
    if (isNaN(minutes)) {
      return "";
    }

    const hours = Math.floor(minutes / 60); 
    const remainingMinutes = minutes % 60; 

    if (hours === 0) {
      return `${remainingMinutes} min`; 
      // return `${remainingMinutes} мин`; 
    } else if (remainingMinutes === 0) {
      return `${hours} h`;
      // return `${hours} ч`;
    } else {
			return `${hours} h ${remainingMinutes} min`; 
			// return `${hours} ч ${remainingMinutes} мин`; 
    }
  }
}
