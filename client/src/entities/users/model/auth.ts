export interface Login {
  email: string;
  password: string;
}

export interface Registration {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
}

export interface Auth {
	accessToken: string | null,
	refreshToken: string | null
}