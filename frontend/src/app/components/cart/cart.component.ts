import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CartProduct } from '../../models/cartProduct';
import { Product } from '../../models/product';
import { forkJoin } from 'rxjs';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';


@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {

  cart: CartProduct[] = [];
  products: Product[] = [];
  totalItems: number = 0;
  quantity!: number;

  constructor(private productService: ProductService, private orderService: OrderService, private router: Router,private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.cart = this.productService.cart;
    this.loadProducts();
    this.calculateTotalItems();
  }

  loadProducts(): void {
    const observables = this.cart.map(item => this.productService.getProductById(item.productId));
    forkJoin(observables).subscribe(products => {
      this.products = products;
    });
  }

  addToCart(id: string): void {
    this.productService.getProductById(id).subscribe( product =>
    {
      if (this.productService.getCartItemAmount(id) >= product.quantity) {
        this.snackBar.open('Amount exceeds product quantity', 'Close', {
          duration: 3000, // Duration in milliseconds
        });
      } else {
      this.productService.addToCart(id);
      this.loadProducts();
      this.calculateTotalItems();
      }
    });
  }
  removeFromCart(id: string): void {
    this.productService.removeFromCart(id);
    this.loadProducts();
    this.calculateTotalItems();
  }

  calculateTotalItems(): void {
    this.totalItems = this.productService.getCartItemCount();
  }

  getTotalPrice(): number {
    let totalPrice = 0;
    for (let i = 0; i < this.cart.length; i++) {
      const product = this.products.find(p => p.productId === this.cart[i].productId);
      if (product) {
        totalPrice += product.price * this.cart[i].amount;
      }
    }
    return totalPrice;
  }

  makeOrder(): void {
    this.orderService.createOrder(this.cart).subscribe({
      next: () => {
        this.router.navigate(['/orders']);
      },
      error: (error) => {
        console.error('Order creation failed: ', error);
      }
    });
  }
}
