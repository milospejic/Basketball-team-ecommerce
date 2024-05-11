import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CartProduct } from '../models/cartProduct';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  cart: CartProduct[] = [];
  constructor(private http: HttpClient) { }
    
  getAllProducts(): Observable<any[]>{
    return this.http.get<any[]>("http://localhost:5259/api/product")
  }

  addToCart(id: string){
    const existingProduct = this.cart.find(product => product.productId === id);
    if (existingProduct) {
      existingProduct.amount++;
    } else {
      this.cart.push({ productId: id, amount: 1 });
    }
  }

  removeFromCart(id: string){
    const existingProduct = this.cart.find(product => product.productId === id);
    if (existingProduct) {
      if (existingProduct.amount > 1) {
        existingProduct.amount--;
      } else {
        const index = this.cart.findIndex(product => product.productId === id);
        if (index !== -1) {
          this.cart.splice(index, 1);
        }
      }
    }
  }

  getCartItemCount(): number {
    return this.cart.reduce((total, product) => total + product.amount, 0);
  }
}
