export interface AuthResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  user: UserDto;
}

export interface UserDto {
  id: string;
  email: string;
  fullName: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
  organizationName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}
