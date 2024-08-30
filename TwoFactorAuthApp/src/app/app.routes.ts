import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { Verify2faComponent } from './auth/verify2fa/verify2fa.component';
import { HomeComponent } from './home/home.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  {
    path: 'verify-2fa',
    component: Verify2faComponent,
  },
  { 
    path: '', 
    redirectTo: '/login', 
    pathMatch: 'full' 
  },
  {
    path:'home',
    component: HomeComponent,
    canActivate:[authGuard]
  }
  // {
  //   path: 'home',
  //   component:HomeComponent,
  //   canActivate:[authGuard]
  // }
];
