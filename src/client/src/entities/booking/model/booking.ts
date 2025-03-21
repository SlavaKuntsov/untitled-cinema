export interface SessionSeats{
	id: string,
	sessionId: string,
	availableSeats: Seat[] | null,
	reservedSeats: Seat[] | null,
	updatedAt: string
}

export interface Seat{
	id: string,
	row: number,
	column: number,
}