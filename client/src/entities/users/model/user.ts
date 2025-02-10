import { DecimalPipe } from "@angular/common";

export interface User {
  email: string;
  role: string;
  firstName: string;
  lastName: string;
  balance: number | DecimalPipe;
  dateOfBirth: Date;
}
