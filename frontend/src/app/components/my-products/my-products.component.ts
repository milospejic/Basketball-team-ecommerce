import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CurrentUser } from '../../models/currentUser';
import { ProductService } from '../../services/product.service';
import { ProductUpdate } from '../../models/productUpdate';

@Component({
  selector: 'app-my-products',
  templateUrl: './my-products.component.html',
  styleUrls: ['./my-products.component.css']
})
export class MyProductsComponent implements OnInit {


  adminId = "try";
  productList: any[] = [];
  isUpdating: boolean = false;
  isAdding: boolean = false;
  productCreate: ProductUpdate = new ProductUpdate();
  updatedProduct: any = {};

  constructor(
    private authService: AuthService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.authService.getCurrentUser().subscribe((user: CurrentUser) => {
      this.adminId = user.id;
      this.productService.getProductsByAdminId(this.adminId).subscribe((result: any) => {
        this.productList = result;
      }, (error) => {
        console.error('Error loading products:', error);
      });
    });
  }

  add() {
    this.isAdding=true;
    this.isUpdating=false;
  }

  update() {
    this.isAdding=false;
    this.isUpdating=true;
  }
  updateProduct(product: any): void {
    this.productService.updateProduct(product.productId, product).subscribe(() => {
      this.isUpdating = false;
      this.loadProducts();
    });
  }

  createProduct() {
    this.productService.createProduct(this.productCreate).subscribe(() => {
      this.isAdding = false;
      this.loadProducts();
    });
    }

  deleteProduct(productId: string): void {
    if (confirm('Are you sure you want to delete this product?')) {
    this.productService.deleteProduct(productId).subscribe(() => {
      this.loadProducts();
    });
  }
  }

  cancelUpdate(): void {
    this.isUpdating = false;
  }
}
