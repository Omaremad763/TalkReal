export interface RegistrationDto {
  username: string;
  password: string;
  email: string;
}
export interface LoginDto {
  email: string;
  password: string;
}
export interface TokenDto {
  accessToken: string;
  refreshToken: string;
}
