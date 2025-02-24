import { DecimalPipe } from "@angular/common";
import { HallDto } from "../../hall/model/hall";

export interface Session {
  id: string;
  movieId: string;
  hall: HallDto;
	priceModifier: number
  startTime: string;
  endTime: string;
}
