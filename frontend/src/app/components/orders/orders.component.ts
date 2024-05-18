import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order';
import { AuthService } from '../../services/auth.service'; 
import { UserService } from '../../services/user.service';
import { CurrentUser } from '../../models/currentUser';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit {

  orders: Order[] = [];

  constructor(private orderService: OrderService, private authService: AuthService, private userService: UserService) { }

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
      // Remove the order from the list
      this.orders = this.orders.filter(order => order.orderId !== orderId);
    });
  }
}
