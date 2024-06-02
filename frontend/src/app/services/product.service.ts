import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CartProduct } from '../models/cartProduct';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  cart: CartProduct[] = [];
  constructor(private http: HttpClient) { }
    
  getAllProducts(): Observable<any[]>{
    return this.http.get<any[]>("http://localhost:5259/api/product")
  }

  getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`http://localhost:5259/api/product/${id}`);
  }

  createProduct(product: Product): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.post<any>('http://localhost:5259/api/product', product, { headers });
  }
  updateProduct(product: Product): Observable<any>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.put<any>(`http://localhost:5259/api/product/${product.productId}`, product, {headers});
  }

  deleteProduct(id: string): Observable<any>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.delete<any>(`http://localhost:5259/api/product/${id}`, {headers});
  }

  getProductsByAdminId(id: string): Observable<Product> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<Product>(`http://localhost:5259/api/product/admin/${id}`, {headers });
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
