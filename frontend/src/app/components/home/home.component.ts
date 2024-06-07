import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private productService: ProductService, private router: Router) {}

  productList: Product[] = [];

  ngOnInit(): void {
    this.loadAllProducts();
  }

  loadAllProducts(): void {
    this.productService.getAllProducts().subscribe(
      (result: Product[]) => {
        this.productList = result;
      },
      (error) => {
        console.error('Error loading products:', error);
      }
    );
  }

  loadProductsByCategory(category: string): void {
    this.productService.getAllProducts().subscribe(
      (result: Product[]) => {
        this.productList = result.filter((product) => product.category === category);
      },
      (error) => {
        console.error('Error loading products:', error);
      }
    );
  }

  addProductToCart(id: string): void {
    this.productService.addToCart(id);
    console.log('Product added to cart:', id);
  }

  removeProductFromCart(id: string): void {
    this.productService.removeFromCart(id);
    console.log('Product removed from cart:', id);
  }

  viewProductDetails(id: string): void {
    this.router.navigate(['/product', id]);
  }
}
