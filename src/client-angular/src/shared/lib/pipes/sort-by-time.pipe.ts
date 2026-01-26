import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "sortByTime",
})
export class SortByTimePipe implements PipeTransform {
  transform(sessions: any[]): any[] {
    if (!sessions) return [];
    return sessions.sort(
      (a, b) =>
        new Date(a.startTime).getTime() - new Date(b.startTime).getTime(),
    );
  }
}
