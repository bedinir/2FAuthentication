import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../auth/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  if(inject(AuthService).isAuthenticated()){
    return true
  }
  console.log('You have to login first')
  return false;
};
