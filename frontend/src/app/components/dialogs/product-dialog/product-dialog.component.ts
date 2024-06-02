import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
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

  constructor(
    public snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<ProductDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Product,
    public productService: ProductService
  ) {}

  public add():void{
    this.productService.createProduct(this.data).subscribe(
      () => {
        this.snackBar.open('Product added', 'Ok', {duration:4500});
      }
    ),
    (error: Error)=> {
      console.log(error.name + ' ' + error.message);
      this.snackBar.open('Dogodila se greska', 'Ok',{duration:2500});
    }
    
  }

  public update(): void {
    this.productService.updateProduct(this.data).subscribe(
      () => {
        this.snackBar.open('Product was successfully updated', 'Ok', { duration: 4500 });
      },
      (error: Error) => {
        console.log(error.name + ' ' + error.message);
        this.snackBar.open('An error has occurred', 'Ok', { duration: 2500 });
      }
    );
  }

  public delete(): void {
    this.productService.deleteProduct(this.data.productId).subscribe(
      () => {
        this.snackBar.open('Product was deleted', 'Ok', { duration: 4500 });
      },
      (error: Error) => {
        console.log(error.name + ' ' + error.message);
        this.snackBar.open('An error has occurred', 'Ok', { duration: 2500 });
      }
    );
  }

  public cancel(): void {
    this.dialogRef.close();
    this.snackBar.open('You canceled!', 'Ok', { duration: 4500 });
  }
}
