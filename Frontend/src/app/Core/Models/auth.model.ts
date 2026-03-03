export interface LoginRequest {
  email: string;
  password: string;
}

export interface SignupRequest {
  fullname: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  token: string | null;
  refreshToken: string | null;
  email: string | null;
  userId: string | null;
}

export interface User {
  userId: string;
  email: string;
  fullName: string;
  role: string;
}

export interface AuthState {
  isAuthenticated: boolean;
  user: User | null;
  token: string | null;
  refreshToken: string | null;
}
