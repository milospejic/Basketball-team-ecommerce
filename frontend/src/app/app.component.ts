import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ProductService } from './services/product.service';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';
import { UserService } from './services/user.service';
import { CurrentUser } from './models/currentUser';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'KK IT Novi Sad';
  isLoggedIn = false;
  isUser = false;

  constructor(
    private productService: ProductService, 
    private authService: AuthService,
    private userService: UserService, 
    private router: Router
  ) {}
  

  ngOnInit(): void {
    this.checkLoginStatus();
  }
  

  private checkLoginStatus(): void {
    console.log(this.isLoggedIn);
    this.isLoggedIn = this.authService.isLoggedIn();
    console.log(this.isLoggedIn);
    if(this.isLoggedIn){
      this.checkUserRole();
    }

  }

  private checkUserRole(): void {
    this.authService.getCurrentUser().subscribe((user: CurrentUser) => {
      console.log(user.role);
      if(user.role == "User"){
        this.isUser = true;
      }else{
        this.isUser = false;
      }
    });
  }

  getCartCount(): number {
    return this.productService.getCartItemCount();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
    this.checkLoginStatus();  // Update the login status after logging out
  }
}
