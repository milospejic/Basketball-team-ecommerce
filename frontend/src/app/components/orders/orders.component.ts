import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order';
import { AuthService } from '../../services/auth.service'; 
import { UserService } from '../../services/user.service';
import { CurrentUser } from '../../models/currentUser';
import { Router } from '@angular/router';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit {

  orders: Order[] = [];

  constructor(
    private orderService: OrderService, 
    private authService: AuthService, 
    private userService: UserService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe((user: CurrentUser) => {
      const userId = user.id;
      this.orderService.getOrdersByUser(userId).subscribe(orders => {
        this.orders = orders;
      });
    });
  }

  deleteOrder(orderId: string): void {
    this.orderService.deleteOrder(orderId).subscribe(() => {
      this.orders = this.orders.filter(order => order.orderId !== orderId);
    });
  }

  redirectToCheckout(orderId: string, fullPrice: number): void {
    this.router.navigate(['/checkout', { orderId, fullPrice }]);
  }
}
