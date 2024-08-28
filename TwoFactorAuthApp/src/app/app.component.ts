import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavBarComponent } from "./core/nav-bar/nav-bar.component";
import { LoginComponent } from "./auth/login/login.component";
import { RegisterComponent } from "./auth/register/register.component";
import { Verify2faComponent } from "./auth/verify2fa/verify2fa.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavBarComponent, LoginComponent, RegisterComponent, Verify2faComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'TwoFactorAuthApp';
}
