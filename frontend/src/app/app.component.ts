import { Component, OnInit } from '@angular/core';
import { ProductService } from './services/product.service';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';
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
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.isLoggedIn.subscribe(status => {
      this.isLoggedIn = status;
      if (this.isLoggedIn) {
        this.checkUserRole();
      }
    });
  }

  private checkUserRole(): void {
    this.authService.getCurrentUser().subscribe((user: CurrentUser) => {
      this.isUser = user.role === "User";
    });
  }

  getCartCount(): number {
    return this.productService.getCartItemCount();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  userRole(): string | null {
    return localStorage.getItem('userRole');
  }
}
