import { DecimalPipe } from "@angular/common";

export interface User {
	id: string,
  email: string;
  role: string;
  firstName: string;
  lastName: string;
  balance: number | DecimalPipe;
  dateOfBirth: string;
}

export interface UpdateUser {
	id: string,
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}