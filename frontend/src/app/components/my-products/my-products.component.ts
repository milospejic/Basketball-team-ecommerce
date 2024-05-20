import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CurrentUser } from '../../models/currentUser';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-my-products',
  templateUrl: './my-products.component.html',
  styleUrl: './my-products.component.css'
})
export class MyProductsComponent implements OnInit {
deleteProduct(id: string) {
throw new Error('Method not implemented.');
}
updateProduct(id: string) {
throw new Error('Method not implemented.');
}
  adminId = "try";
  productList: any[]  = []; 
  constructor(private authService: AuthService, private productService: ProductService){}
  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe((user: CurrentUser) => {
      this.adminId = user.id;
      this.productService.getProductsByAdminId(this.adminId).subscribe((result: any)=>{
        this.productList = result;
        //console.log('Products loaded:', this.productList);
      }, (error) => {
        console.error('Error loading products:', error);
      });
    });
  }

  
  
}
