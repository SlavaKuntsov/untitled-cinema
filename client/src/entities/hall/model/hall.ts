export interface Hall {
  id: string;
  name: string;
  totalSeats: number;
  seatsArray: number[][];
}

export interface HallDto {
  hallId: string;
  hallName: string;
}
