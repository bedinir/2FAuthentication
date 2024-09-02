import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ProfileComponent } from "./profile/profile.component";
import { UserDetailsComponent } from "./user-details/user-details.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule, ProfileComponent, UserDetailsComponent,CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  currentSection: string = 'home'; // Default section

  showSection(section: string) {
    this.currentSection = section;
  }
}