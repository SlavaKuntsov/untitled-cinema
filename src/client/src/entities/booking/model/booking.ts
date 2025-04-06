import { Seat } from "../../seat";

export interface SessionSeats {
  id: string;
  sessionId: string;
  availableSeats: Seat[] | null;
  reservedSeats: Seat[] | null;
  updatedAt: string;
}

export interface Booking {
	userId: string;
	sessionId: string;
	seats: Seat[]
}