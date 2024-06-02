import { Component } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { Order } from '../../models/order';

@Component({
  selector: 'app-all-orders',
  templateUrl: './all-orders.component.html',
  styleUrl: './all-orders.component.css'
})
export class AllOrdersComponent {

  orders: Order[] = [];
  
  constructor(private orderService: OrderService, private authService: AuthService, private userService: UserService) { }

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.orderService.getAllOrders().subscribe(orders => {
      this.orders = orders;
    });
  }
  updateStatus(orderId: string): void {
    this.orderService.setStatusSent(orderId).subscribe(() => {
      this.loadOrders();
    });
  }

  deleteOrder(orderId: string): void {
    this.orderService.deleteOrder(orderId).subscribe(() => {
      this.orders = this.orders.filter(order => order.orderId !== orderId);
    });
  }
}
