import { Routes } from '@angular/router';
import { LoginComponent } from '../Pages/Auth/login/login';
import { RegisterComponent } from '../Pages/Auth/register/register';
export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
];
