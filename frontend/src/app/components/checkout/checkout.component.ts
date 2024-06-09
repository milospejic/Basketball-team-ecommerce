import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { PaymentService } from '../../services/payment.service';
import { ActivatedRoute } from '@angular/router';
import { loadStripe, Stripe, StripeElements, StripeCardElement } from '@stripe/stripe-js';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements AfterViewInit {
  stripe!: Stripe;
  elements!: StripeElements;
  cardElement!: StripeCardElement;
  clientSecret!: string;
  orderId!: string | null;
  fullPrice!: string | null;

  @ViewChild('cardElement') cardElementRef!: ElementRef;

  constructor(private paymentService: PaymentService, private route: ActivatedRoute) {}

  async ngAfterViewInit(): Promise<void> {
    this.orderId = this.route.snapshot.paramMap.get('orderId');
    this.fullPrice = this.route.snapshot.paramMap.get('fullPrice');
    await this.initializeStripe();
    await this.createPaymentIntent();
  }

  async initializeStripe(): Promise<void> {
    const stripe = await loadStripe(environment.stripe.publishableKey);
    if (stripe) {
      this.stripe = stripe;
      this.elements = this.stripe.elements();
      this.cardElement = this.elements.create('card');
      this.cardElement.mount(this.cardElementRef.nativeElement);
    } else {
      console.error('Stripe failed to load');
    }
  }

  async createPaymentIntent(): Promise<void> {
    if (this.fullPrice == null || this.orderId == null) {
      console.log('Payment failed');
    } else {
      const amount = parseFloat(this.fullPrice) * 100;
      const currency = 'usd';

      this.paymentService.createPaymentIntent(amount, currency, this.orderId).subscribe(response => {
        this.clientSecret = response.clientSecret;
        this.setupPaymentForm();
      });
    }
  }

  setupPaymentForm(): void {
    const form = document.getElementById('payment-form');
    if (form) {
      form.addEventListener('submit', async (event) => {
        event.preventDefault();
        await this.handlePayment();
      });
    }
  }

  async handlePayment(): Promise<void> {
    const { error, paymentIntent } = await this.stripe.confirmCardPayment(this.clientSecret, {
      payment_method: {
        card: this.cardElement,
        billing_details: {
          name: 'Customer Name'
        }
      }
    });

    if (error) {
      console.error('Payment failed', error);
    } else if (paymentIntent?.status === 'succeeded') {
      console.log('Payment succeeded', paymentIntent);
    }
  }
}
