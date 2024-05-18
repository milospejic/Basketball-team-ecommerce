import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CurrentUser } from '../../models/currentUser';

@Component({
  selector: 'app-my-products',
  templateUrl: './my-products.component.html',
  styleUrl: './my-products.component.css'
})
export class MyProductsComponent implements OnInit {
  userId = "try"
  constructor(private authService: AuthService){}
  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe((user: CurrentUser) => {
      this.userId = user.id;
    });
  }

  
  
}
