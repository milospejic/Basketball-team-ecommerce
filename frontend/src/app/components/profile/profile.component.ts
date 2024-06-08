import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  user: User = new User();
  isUpdating: boolean = false;

  constructor(private userService: UserService, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe(currentUser =>{
      this.userService.getUserById(currentUser.id).subscribe(user => {
        this.user = user;
      });
    });
  }

  updateUser(): void {
    this.userService.updateUser(this.user).subscribe(() => {
      this.isUpdating = false;
    });
  }
/*
  deleteUser(): void {
    this.userService.deleteUser(this.user.userId).subscribe(() => {
      this.router.navigate(['/login']); 
    });
  }
    */
}