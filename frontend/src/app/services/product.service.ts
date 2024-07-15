import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CartProduct } from '../models/cartProduct';
import { Product } from '../models/product';
import { ProductUpdate } from '../models/productUpdate';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  
  cart: CartProduct[] = [];
  constructor(private http: HttpClient) { }
    
  getAllProductsFull() : Observable<any[]>{
    return this.http.get<any[]>("https://localhost:7261/api/product");
  }
  getAllProducts(page: number, pageSize: number, sortBy: string, sortOrder: string): Observable<Product[]> {
    const url = `https://localhost:7261/api/product?page=${page}&pageSize=${pageSize}&sortBy=${sortBy}&sortOrder=${sortOrder}`;
    return this.http.get<Product[]>(url);
  }

  getAllProductsByCategory(id: string): Observable<any[]>{
    return this.http.get<any[]>("https://localhost:7261/api/product/category");
  }

  getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`https://localhost:7261/api/product/${id}`);
  }

  createProduct(product: ProductUpdate): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.post<any>('https://localhost:7261/api/product', product, { headers });
  }
  updateProduct(id: String ,product: ProductUpdate): Observable<any>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.put<any>(`https://localhost:7261/api/product/${id}`, product, {headers});
  }

  deleteProduct(id: string): Observable<any>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.delete<any>(`https://localhost:7261/api/product/${id}`, {headers});
  }

  getProductsByAdminId(id: string): Observable<Product> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<Product>(`https://localhost:7261/api/product/admin/${id}`, {headers });
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

  searchProducts(query: string): Observable<Product[]> {
    return this.http.get<Product[]>(`https://localhost:7261/api/product/search?query=${query}`);
  }

  getCartItemAmount(productId: string): number {
    const cartItem = this.cart.find(item => item.productId === productId);
    return cartItem ? cartItem.amount : 0;
  }
}
