import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Product } from '../../../models/product';
import { ProductService } from '../../../services/product.service';

@Component({
  selector: 'app-product-dialog',
  templateUrl: './product-dialog.component.html',
  styleUrls: ['./product-dialog.component.css']
})
export class ProductDialogComponent {
  flag!: number;
  productForm: FormGroup;

constructor(
  public snackBar: MatSnackBar,
  public dialogRef: MatDialogRef<ProductDialogComponent>,
  @Inject(MAT_DIALOG_DATA) public data: Product,
  private fb: FormBuilder,
  private productService: ProductService
) {
  this.productForm = this.fb.group({
    productName: [data.productName, Validators.required],
    description: [data.description, Validators.required],
    image: [data.image, Validators.required],
    brand: [data.brand],
    category: [data.category, Validators.required],
    size: [data.size],
    price: [data.price, [Validators.required, Validators.pattern(/^[0-9]+(\.[0-9]{1,2})?$/)]],
    quantity: [data.quantity, [Validators.required, Validators.pattern(/^[0-9]+$/)]],
    discountId: [data.discountId]
  });
}
  public add(): void {
    this.productService.createProduct(this.productForm.value).subscribe(
      () => {
        this.snackBar.open('Product added', 'Ok', { duration: 4500 });
        this.dialogRef.close({ action: 'create' });
      },
      (error: Error) => {
        console.log(error.name + ' ' + error.message);
        this.snackBar.open('An error occurred', 'Ok', { duration: 2500 });
      }
    );
  }

  public update(): void {
    this.productService.updateProduct(this.data.productId,this.productForm.value).subscribe(
      () => {
        this.snackBar.open('Product updated', 'Ok', { duration: 4500 });
        this.dialogRef.close({ action: 'update' });
      },
      (error: Error) => {
        console.log(error.name + ' ' + error.message);
        this.snackBar.open('An error occurred', 'Ok', { duration: 2500 });
      }
    );
  }

  public delete(): void {
    // Add a confirmation dialog before deleting
    const confirmDelete = confirm('Are you sure you want to delete this product?');
    if (confirmDelete) {
      this.productService.deleteProduct(this.data.productId).subscribe(
        () => {
          this.snackBar.open('Product deleted', 'Ok', { duration: 4500 });
          this.dialogRef.close({ action: 'delete' });
        },
        (error: Error) => {
          console.log(error.name + ' ' + error.message);
          this.snackBar.open('An error occurred', 'Ok', { duration: 2500 });
        }
      );
    }
  }

  public cancel(): void {
    this.dialogRef.close();
    this.snackBar.open('You canceled!', 'Ok', { duration: 4500 });
  }
}
