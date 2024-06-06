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

  getAllOrders(): Observable<any[]>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<any[]>("https://localhost:7261/api/order", {headers})
  }
  
  createOrder(cart: CartProduct[]): Observable<any> {
    this.productsInOrder.productsInOrder = cart;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.post<any>('https://localhost:7261/api/order', this.productsInOrder, { headers });
  }

  deleteOrder(orderId: string): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.delete(`https://localhost:7261/api/order/${orderId}`, { headers });
  }

  setStatusSent(id: string):Observable<Order>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.patch<Order>(`https://localhost:7261/api/order/sent/${id}`, { headers });

  }

  getOrdersByUser(userId: string): Observable<Order[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<Order[]>(`https://localhost:7261/api/order/user/${userId}`, { headers });
  }
}
