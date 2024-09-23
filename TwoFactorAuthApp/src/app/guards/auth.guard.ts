import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  if (inject(AuthService).isAuthenticated()) {
    console.log('isAuth')
    return true;
  } else {
    inject(Router).navigate(['login']);
  }

  // inject(Router).navigate(['login']);

  return false;
};
