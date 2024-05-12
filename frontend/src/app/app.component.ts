import { Component, OnChanges, OnInit } from '@angular/core';
import { ProductService } from './services/product.service';
import { Router } from '@angular/router';
import { UserService } from './services/user.service';
import { AdminService } from './services/admin.service';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, OnChanges{
  title = 'KK IT Novi Sad';
  isLoggedIn!: boolean;
  constructor(private productService: ProductService, private authService: AuthService){
    
  }
  ngOnInit(): void {
    this.authService.isLoggedIn().subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
    });
  }

  ngOnChanges(): void {
    this.authService.isLoggedIn().subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
    });
  }

  getCartCount(): number{
    return this.productService.getCartItemCount();
  }

  
  
}
