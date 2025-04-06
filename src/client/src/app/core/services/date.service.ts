import { Injectable, signal } from "@angular/core";

@Injectable({
	providedIn: "root",
})
export class DateService {
		private _selectedDate = signal<Date | null>(null);
		public selectedDate = this._selectedDate.asReadonly();
	
		updateSeatsArray(date: Date): void {
			this._selectedDate.set(date);
		}
}