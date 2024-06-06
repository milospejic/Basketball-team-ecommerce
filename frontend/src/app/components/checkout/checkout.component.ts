import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { PaymentService } from '../../services/payment.service';
import { ActivatedRoute } from '@angular/router';
import { loadStripe } from '@stripe/stripe-js';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements AfterViewInit {
  stripe: any;
  clientSecret!: string;
  orderId!: string | null;
  fullPrice!: string | null;
  @ViewChild('cardElement') cardElement!: ElementRef;

  constructor(private paymentService: PaymentService, private route: ActivatedRoute) {}

  ngAfterViewInit(): void {
    this.orderId = this.route.snapshot.paramMap.get('orderId');
    this.fullPrice = this.route.snapshot.paramMap.get('fullPrice');
    this.initializeStripe();
    this.createPaymentIntent();
  }

  async initializeStripe() {
    this.stripe = await loadStripe(environment.stripe.publishableKey);
    const elements = this.stripe.elements();
    const cardElement = elements.create('card');
    cardElement.mount(this.cardElement.nativeElement);
    console.log('done');
  }

  async createPaymentIntent() {
    if (this.fullPrice == null || this.orderId == null) {
      console.log('Payment failed');
      return;
    }

    const amount = parseFloat(this.fullPrice);
    const currency = 'usd';
    this.paymentService.createPaymentIntent(amount, currency, this.orderId).subscribe(async (response) => {
      this.clientSecret = response.clientSecret;
      const form = document.getElementById('payment-form');
      if (form != null) {
        form.addEventListener('submit', async (event) => {
          event.preventDefault();
          const { error } = await this.stripe.confirmCardPayment(this.clientSecret, {
            payment_method: {
              card: this.cardElement.nativeElement,
              billing_details: {
                name: 'Customer Name'
              }
            }
          });
          if (error) {
            console.error('Payment failed', error);
          } else {
            console.log('Payment succeeded');
          }
        });
      }
    });
  }
}