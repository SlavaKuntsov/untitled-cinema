export interface Login {
  email: string;
  password: string;
}

export interface Auth {
	accessToken: string | null,
	refreshToken: string | null
}