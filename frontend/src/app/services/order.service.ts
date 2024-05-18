import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CartProduct } from '../models/cartProduct';
import { Observable } from 'rxjs';
import { ProductsInOrder } from '../models/productsInOrder';
import { Order } from '../models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private http: HttpClient) { }

  productsInOrder: ProductsInOrder = new ProductsInOrder();
  
  createOrder(cart: CartProduct[]): Observable<any> {
    console.log("usao");
    console.log(localStorage.getItem('token'));
    this.productsInOrder.productsInOrder = cart;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.post<any>('http://localhost:5259/api/order', this.productsInOrder, { headers });
  }

  deleteOrder(orderId: string): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.delete(`http://localhost:5259/api/order/${orderId}`, { headers });
  }

  getOrdersByUser(userId: string): Observable<Order[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<Order[]>(`http://localhost:5259/api/order/user/${userId}`, { headers });
  }
}
