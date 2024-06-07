import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { CartComponent } from './components/cart/cart.component';
import { SaleComponent } from './components/sale/sale.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ContactusComponent } from './components/contactus/contactus.component';
import { OrdersComponent } from './components/orders/orders.component';
import { AllOrdersComponent } from './components/all-orders/all-orders.component';
import { ProfileComponent } from './components/profile/profile.component';
import { AddressComponent } from './components/address/address.component';
import { MyProductsComponent } from './components/my-products/my-products.component';
import { CheckoutComponent } from './components/checkout/checkout.component';
import { ProductDetailsComponent } from './components/product-details/product-details.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: "full",
    component: HomeComponent
  },{
    path: 'home',
    component: HomeComponent
  },{
    path: 'cart',
    component: CartComponent
  },{
    path: 'sale',
    component: SaleComponent
  },{
    path: 'login',
    component: LoginComponent
  },{
    path: 'register',
    component: RegisterComponent
  },{
    path: 'contactus',
    component: ContactusComponent
  },{
    path: 'orders',
    component: OrdersComponent
  },{
    path: 'all-orders',
    component: AllOrdersComponent
  },{
    path: 'profile',
    component: ProfileComponent
  },{
    path: 'address',
    component: AddressComponent
  },{
    path: 'my-products',
    component: MyProductsComponent
  },{
    path: 'checkout',
    component: CheckoutComponent
  },{
    path: 'product/:id',
    component: ProductDetailsComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
