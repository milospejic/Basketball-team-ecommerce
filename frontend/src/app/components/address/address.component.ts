import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { Address } from '../../models/address';
import { AddressService } from '../../services/address.service';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';

@Component({
  selector: 'app-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.css']
})
export class AddressComponent implements OnInit {
  user: User = new User();
  address: Address = new Address();
  isUpdating: boolean = false;
  isAdding: boolean = false;

  constructor(private addressService: AddressService, private userService: UserService, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe(currentUser => {
      this.userService.getUserById(currentUser.id).subscribe(user => {
        this.user = user;
        if (user.addressId != '00000000-0000-0000-0000-000000000000') {
          this.addressService.getAddressById(user.addressId).subscribe(address => {
            this.address = address;
          });
        } else {
          this.isAdding = true;
        }
      });
    });
  }

  updateAddress(): void {
    this.addressService.updateAddress(this.address).subscribe(() => {
      this.isUpdating = false;
    });
  }

  deleteAddress(): void {
    this.addressService.deleteAddress(this.address.addressId).subscribe(() => {
      this.router.navigate(['/address']); 
    });
  }

  addAddress(): void {
    this.addressService.addAddress(this.address).subscribe(newAddress => {
      this.user.addressId = newAddress.addressId;
      this.userService.updateUser(this.user).subscribe(() => {
        this.isAdding = false;
        this.router.navigate(['/profile']);
      });
    });
  }
}
