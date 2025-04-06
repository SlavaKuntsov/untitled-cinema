import { Seat } from "../../seat";

export interface SessionSeats {
  id: string;
  sessionId: string;
  availableSeats: Seat[] | null;
  reservedSeats: Seat[] | null;
  updatedAt: string;
}

export interface Booking {
  id: string;
  userId: string;
  sessionId: string;
  seats: Seat[];
  totalPrice: number;
  status: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface BookingDto {
  userId: string;
  sessionId: string;
  seats: Seat[];
}
