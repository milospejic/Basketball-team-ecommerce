import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{

  constructor(private productService: ProductService){
    
  }

  productList: any[]  = []; 
  productsInCart: string[] = []
  ngOnInit(): void {
    this.loadAllProducts();
  }
  loadAllProducts() {
    //console.log('Loading all products...');
    this.productService.getAllProducts().subscribe((result: any)=>{
      this.productList = result;
      //console.log('Products loaded:', this.productList);
    }, (error) => {
      console.error('Error loading products:', error);
    });
  }

  addProductToCart(id: string){
    this.productService.addToCart(id);
    console.log('Product added to cart:', id);
  }

  removeProductFromCart(id: string){
    this.productService.removeFromCart(id);
    console.log('Product removed from cart:', id);
  }
}
