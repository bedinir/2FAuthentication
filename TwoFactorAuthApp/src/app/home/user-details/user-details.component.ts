import { Component } from '@angular/core';
import { HomeComponent } from "../home.component";
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './user-details.component.html',
  styleUrl: './user-details.component.css'
})
export class UserDetailsComponent {

}
