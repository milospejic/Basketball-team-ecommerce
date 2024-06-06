import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CurrentUser } from '../../models/currentUser';
import { ProductService } from '../../services/product.service';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ProductDialogComponent } from '../dialogs/product-dialog/product-dialog.component';

@Component({
  selector: 'app-my-products',
  templateUrl: './my-products.component.html',
  styleUrls: ['./my-products.component.css']
})
export class MyProductsComponent implements OnInit {
  adminId = "try";
  productList: any[]  = [];

  constructor(
    private authService: AuthService,
    private productService: ProductService,
    private dialog: MatDialog
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

  public openDialog(
    flag: number,
    productId?: string,
    productName?: string,
    description?: string,
    image?: string,
    brand?: string,
    category?: string,
    size?: string,
    price?: number,
    quantity?: number,
    discountId?: string
  ): void {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.hasBackdrop = true; // Set this to make the dialog modal
    dialogConfig.data = {
      productId,
      productName,
      description,
      image,
      brand,
      category,
      size,
      price,
      quantity,
      discountId
    };
  
    const dialogRef = this.dialog.open(ProductDialogComponent, dialogConfig);
    dialogRef.componentInstance.flag = flag;
    dialogRef.afterClosed().subscribe(result => {
      if (result == 1) {
        this.loadProducts();
      }
    });
  }
}
