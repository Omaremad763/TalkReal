import { Routes } from '@angular/router';
import { LoginComponent } from '../Pages/Auth/login/login';
import { RegisterComponent } from '../Pages/Auth/register/register';
import { ChatPageComponent } from '../Pages/Chat UI/chatpage';
import { authGuard } from '../shared/guards/auth.guard';
import { MainLayoutComponent } from '../shared/layout/MainLayout';
export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [{ path: 'ChatPage', component: ChatPageComponent }],
  },
  { path: '**', redirectTo: 'login' },
];
