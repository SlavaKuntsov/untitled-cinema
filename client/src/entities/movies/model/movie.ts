import { DecimalPipe } from "@angular/common";

export interface Movie {
  id: string;
	title: string,
	genres: string[],
	description: string,
	poster: string
	price: number | DecimalPipe,
	durationMinutes: number,
	producer: string,
	inRoles: string,
	ageLimit: number,
	releaseDate: string,
	createdAt: string,
	updatedAt: string,
}

export interface Genre{
	id: string,
	name: string
}