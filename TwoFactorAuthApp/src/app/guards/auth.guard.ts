import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  if (inject(AuthService).isAuthenticated()) {
    console.log('isAuth')
    return true;
  }

  // inject(Router).navigate(['login']);

  return false;
};
