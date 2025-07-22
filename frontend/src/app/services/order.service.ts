import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CartProduct } from '../models/cartProduct';
import { Observable } from 'rxjs';
import { ProductsInOrder } from '../models/productsInOrder';
import { Order } from '../models/order';
import { environment } from '../environments/environment';

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
    return this.http.get<any[]>(`${environment.apiUrl}/api/order`, {headers})
  }
  
  createOrder(cart: CartProduct[]): Observable<any> {
    this.productsInOrder.productsInOrder = cart;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.post<any>(`${environment.apiUrl}/api/order`, this.productsInOrder, { headers });
  }

  deleteOrder(orderId: string): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.delete(`${environment.apiUrl}/api/order/${orderId}`, { headers });
  }

  setStatusSent(id: string):Observable<Order>{
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.patch<Order>(`${environment.apiUrl}/api/order/sent/${id}`,{}, { headers });

  }

  getOrdersByUser(userId: string): Observable<Order[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    });
    return this.http.get<Order[]>(`${environment.apiUrl}/api/order/user/${userId}`, { headers });
  }
}
