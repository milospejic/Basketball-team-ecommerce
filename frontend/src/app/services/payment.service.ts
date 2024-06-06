import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private paymentUrl = 'https://localhost:7261/api/payment/create-payment-intent';

  constructor(private http: HttpClient) {}

  createPaymentIntent(amount: number, currency: string, orderId: string): Observable<any> {
    return this.http.post(this.paymentUrl, { amount, currency, orderId });
  }
}

