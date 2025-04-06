export interface SeatType {
  id: string;
  name: string;
  priceModifier: number;
}

export interface SelectedSeat extends Seat {
  seatType: SeatType;
  price: number;
}

export interface Seat {
  id: string;
  row: number;
  column: number;
}
