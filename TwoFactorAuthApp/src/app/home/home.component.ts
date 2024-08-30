import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ProfileComponent } from "./profile/profile.component";
import { UserDetailsComponent } from "./user-details/user-details.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule, ProfileComponent, UserDetailsComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
