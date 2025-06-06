import { Component, inject, Inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {
  authService =  inject(AuthService);
  router = inject(Router);

  logout(){
    this.authService.logout();
    this.router.navigate(['login'])
  }
}
