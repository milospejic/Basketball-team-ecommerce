import { Component, Inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Address } from '../../../models/address';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AddressService } from '../../../services/address.service';

@Component({
  selector: 'app-address-dialog',
  templateUrl: './address-dialog.component.html',
  styleUrl: './address-dialog.component.css'
})
export class AddressDialogComponent {


  flag!: number;

  constructor(
    public snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<AddressDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Address,
    public addressService: AddressService
  ) {}

  public add():void{
    this.addressService.addAddress(this.data).subscribe(
      () => {
        this.snackBar.open('User address added', 'Ok', {duration:4500});
      }
    ),
    (error: Error)=> {
      console.log(error.name + ' ' + error.message);
      this.snackBar.open('Dogodila se greska', 'Ok',{duration:2500});
    }
    
  }

  public update(): void {
    this.addressService.updateAddress(this.data).subscribe(
      () => {
        this.snackBar.open('Address was successfully updated', 'Ok', { duration: 4500 });
      },
      (error: Error) => {
        console.log(error.name + ' ' + error.message);
        this.snackBar.open('An error has occurred', 'Ok', { duration: 2500 });
      }
    );
  }

  public delete(): void {
    this.addressService.deleteAddress(this.data.addressId).subscribe(
      () => {
        this.snackBar.open('Address was deleted', 'Ok', { duration: 4500 });
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
