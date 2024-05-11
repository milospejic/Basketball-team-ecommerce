import { Component } from '@angular/core';
import { ProductService } from './services/product.service';
import { Router } from '@angular/router';
import { UserService } from './services/user.service';
import { AdminService } from './services/admin.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent{
  title = 'KK IT Novi Sad';

  constructor(private productService: ProductService, private userService: UserService, private adminService: AdminService){
    
  }

  getCartCount(): number{
    return this.productService.getCartItemCount();
  }

  
}
